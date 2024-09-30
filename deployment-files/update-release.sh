rm -rf /tmp/release-backup
mkdir /tmp/release-backup
mv * /tmp/release-backup
DOWNLOAD_URL=$(curl -s https://api.github.com/repos/pbijdens/centaur-scores-api/releases/latest | grep browser_download_url | cut -d '"' -f4)
curl -sL $DOWNLOAD_URL | tar xvfz -
cp /tmp/release-backup/appsettings.Production.json .