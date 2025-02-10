# Version 1.1.5

Date: 10/2/2025

## New endpoints
Endpoints have been added for GET /match, GET /competition and GET /ruleset to only return items for a specific participant list. These endpoints are  ```GET /list/{listId}/match```, ```GET /list/{listId}/competition``` and ```GET /list/{listId}/ruleset```. We're using the currently selected participant list as the new aggregate root of the data model. All non-configuration data in the system always belongs to a participant-list.

## Increased token timeout
The token timeout duration has been increased to 150 minutes, preventing admins from having to log in again mid-match for most matches.

