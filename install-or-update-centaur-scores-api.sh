#!/bin/bash
systemctl stop centaurscoresapi
sleep 5

DOWNLOAD_URL=$(curl -s https://api.github.com/repos/pbijdens/centaur-scores-api/releases/latest | grep browser_download_url | cut -d '"' -f4)
DB_SERVER="127.0.0.1"
DB_USER="csuser"
DB_NAME="CentaurScores"
DB_PASSWORD="YOUR-DATABASE-PASSWORD"

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
  "ConnectionStrings": {
    "CentaurScoresDatabase": "server=$DB_SERVER;database=$DB_NAME;user=$DB_USER;password=$DB_PASSWORD"
  }
}
EOT

chown -R root:root /var/www/centaurscoresapi
chmod -R a+rX /var/www/centaurscoresapi

systemctl enable centaurscoresapi.service

systemctl start centaurscoresapi
sleep 5
systemctl status centaurscoresapi