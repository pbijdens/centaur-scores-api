# Cenatur scores API

API server for the Centaur scorekeeping software.

## Installation

Requirements:
- MySQL server
- .NET Core 8 Runtime

## Install .NET Core 8

Just install .NET core 8 on your server.

## Configure MYSQL

Create a user:
```sql
CREATE DATABASE CentaurScores;
CREATE USER 'csuser'@'{host}' IDENTIFIED WITH mysql_native_password BY '{superSecretPassword!123}';
GRANT ALL PRIVILEGES ON CentaurScores.* TO 'csuser'@'%';
FLUSH PRIVILEGES;
```

You can now set-up a connection string in ```appsettings.json``` on your deployed instance like this:

```json
  "AppSettings": {
    "Secret": "{any secret will do}",
    "BackupSecret": "{secret string for the backup API}",
    "AdminACLId": 1,
    "DefaultUser": "csadmin",
    "DefaultUserHash": "{password hash generated with powershell script}"
  },  
  {
    ...,
    "ConnectionStrings": {
      "CentaurScoresDatabase": "server={server-ip};database=CentaurScores;user=csuser;password={superSecretPassword!123}"
    }
  }
```

The password has in abocve configuration is used to seed a user when no users are present in the system. It is generated uwing this PowerShell script:

Make sure the SALT parameter is a 4-charcater string, and the secret parameter contains your password.

```PowerShell
param (
    [Parameter(Mandatory = $true)]
    [string]$Salt,
    [Parameter(Mandatory = $true)]
    [string]$Secret
)

function Calculate-Sha256Hash {
    param (
        [Parameter(Mandatory = $true)]
        [string]$InputString
    )

    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($InputString)
        $hashBytes = $sha256.ComputeHash($bytes)
        $hashString = -join ($hashBytes | ForEach-Object { $_.ToString("x2") })
        return $hashString
    }
    finally {
        $sha256.Dispose()
    }
}

$SaltSecret = "$Salt$Secret"
$PWD = "$($Salt)$(Calculate-Sha256Hash -InputString $SaltSecret)"
Write-Output $PWD
```

The first time you use the software, the initial database schema will automatically be created and seeded.

## Deploy the software

Start with creating a user account that is used to run this software. This user needs no special privileges on the system.

```sh
sudo adduser --system csuser
```

Copy over the software to a temporary folder in the system. Then create a folder to host from. Make sure this is *not* owned by the system user you just created, make sure they can access it though.

```sh
sudo mkdir -p /var/www/centaurscoresapi
sudo cp -R /tmp/release /var/www/centaurscoresapi
sudo chown -R root:root /var/www/centaurscoresapi
sudo chmod -R a+rX /var/www/centaurscoresapi
```

If you don't have the file yet, copy /var/www/centaurscoresapi/appsettings.json to /var/www/centaurscoresapi/appsettings.Production.json and fill in the missing information. The Secret can just be some random string, the LoginSecret is an API password for some restricted functionality. *this is currently notr used so set it to some default*.

Create a service definition file for your service, based on ```deployment-files/centaurscoresapi.service```. Note that you will need to adjust this for the location of your dotnet binary.

Store the updated file in ```/etc/systemd/system/centaurscoresapi.service``` then enable it.

```sh
sudo systemctl enable centaurscoresapi.service
```

Then start it and check if it all worked.

```sh
sudo systemctl start centaurscoresapi
sudo systemctl status centaurscoresapi
```

Congratulations, you should now have your API service running on ```http://<server-ip>:8062/```


## Updating

Stop the service:

```sh
sudo systemctl stop centaurscoresapi
```

Copy the new release into the runtime folder:

```sh
sudo rm -f /tmp/release/appsettings.Production.json
sudo cp -R /tmp/release /var/www/centaurscoresapi
sudo chown -R root:root /var/www/centaurscoresapi
sudo chmod -R a+rX /var/www/centaurscoresapi
```

If database updates are required, apply them now to your MySQL database.

Start the software:

```sh
sudo systemctl start centaurscoresapi
```

## Doing it all automatically

```sh
#!/bin/bash
systemctl stop centaurscoresapi
sleep 5

DOWNLOAD_URL=$(curl -s https://api.github.com/repos/pbijdens/centaur-scores-api/releases/latest | grep browser_download_url | cut -d '"' -f4)
DB_SERVER="127.0.0.1"
DB_PORT=3306
DB_USER="csuser"
DB_NAME="CentaurScores"
DB_PASSWORD="CentaurSecret!123"
CS_INITIAL_JWT="Any random string will do, make it unique changethis"
CS_INITIAL_PASSWORD_HASH="YourCalculatedHashAndSaltChangeThis"
CS_BACKUP_SECRET="myBackupSecretChangeThis"

useradd --system --home-dir /var/www --no-create-home csuser --shell /usr/sbin/nologin
mkdir -p /var/www/centaurscoresapi
rm -rf /tmp/release
mkdir /tmp/release
pushd /tmp/release
curl -sL $DOWNLOAD_URL | tar xvfz -
popd
rm -rf /var/www/centaurscoresapi/*
cp -R /tmp/release/* /var/www/centaurscoresapi
chown -R root:root /var/www/centaurscoresapi
chmod -R a+rX /var/www/centaurscoresapi

cat <<EOT > /etc/systemd/system/centaurscoresapi.service
[Unit]
Description=CentaurScores API Service (kestrel)

[Service]
WorkingDirectory=/var/www/centaurscoresapi
ExecStart=/usr/bin/dotnet /var/www/centaurscoresapi/CentaurScores.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=centaurscoresapi
User=csuser
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://*:8062

[Install]
WantedBy=multi-user.target
EOT

cat <<EOT > /var/www/centaurscoresapi/appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AppSettings": {
    "Secret": "$CS_INITIAL_JWT",
    "BackupSecret": "$CS_BACKUP_SECRET",
    "AdminACLId": 1,
    "DefaultUser": "csadmin",
    "DefaultUserHash": "$CS_INITIAL_PASSWORD_HASH"
  },  

  "ConnectionStrings": {
    "CentaurScoresDatabase": "server=$DB_SERVER;port=$DB_PORT;database=$DB_NAME;port=3306;user=$DB_USER;password=$DB_PASSWORD"
  }
}
EOT

chown -R root:root /var/www/centaurscoresapi
chmod -R a+rX /var/www/centaurscoresapi

systemctl enable centaurscoresapi.service

systemctl start centaurscoresapi
sleep 5
systemctl status centaurscoresapi

```
