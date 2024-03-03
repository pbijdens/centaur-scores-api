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
{
  ...,
  "ConnectionStrings": {
    "CentaurScoresDatabase": "server={server-ip};database=CentaurScores;user=csuser;password={superSecretPassword!123}"
  }
}
```

The first time you use the software, the initial database schema will automatically be created.

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