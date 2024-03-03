# Cenatur scores API

API server for the Centaur scorekeeping software.

## Installation

Requirements:
- MySQL server
- .NET Core 8 Runtime

## Deploy the software

{{TODO}}

## Configure MYSQL

Create a user:
```
CREATE DATABASE CentaurScores;
CREATE USER 'csuser'@'{host}' IDENTIFIED WITH mysql_native_password BY '{superSecretPassword!123}';
GRANT ALL PRIVILEGES ON CentaurScores.* TO 'csuser'@'%';
FLUSH PRIVILEGES;
```

You can now set-up a connection string in ```appsettings.json``` on your deployed instance like this:

```
{
  ...,
  "ConnectionStrings": {
    "CentaurScoresDatabase": "server={server-ip};database=CentaurScores;user=csuser;password={superSecretPassword!123}"
  }
}
```

The first time you use the software, the initial database schema will automatically be created.