# Version 1.1.20

Support inactive partiticpants lists. 

Add pure 18M3P competition format.

No longer return any matches or competitions when no list is active.

# Version 1.1.19

Bugfix relase.

# Version 1.1.14

Date: 2025-03-17

Fixed issue where in finals previous rounds would appear to have a zero-score.

# Version 1.1.13

Version sync

# Version 1.1.12

Date: 2025-03-01

## Custom finals

Added support for creating customized finals.

# Version 1.1.10

Date: 2025-02-19

## Head to head knockout finals

Added support for head to head knockout finals. For this additional endpoints were added to the atches controller: ```POST /match/{id}/finals``` to create a final match from an existing match, ```POST /match/{id}/finals/nextround``` to progresss a final to the next round, ```POST /match/{id}/finals/nextround/undo``` to undo progressing to the next round and ```PUT /match/{id}/finals/win/{discipline}/{bracket}/{winner}/{loser}``` to register a result for a single round in a knock-out finals.

In addition, to the ```MatchResultModel``` data structure, a member for ```FinalScores``` was added, that in case of a knock out final type match will contain information about the brackets and round scores for current, previous and next rounds. The active round and number of rounds can be get (and set) in the match propertlies.

The ```ParticipantModel``` classes have been extended with H2HInfo and HeadToHeadJSON members that can be used to access all ehad to head round information for all participants ot a match. This data can't be updated using the regular endpoints, but will be updated as a side-effect of specific head-to-head endpoints only.

Finals are always ignored when creating personal best lists.

Otherwise, all regular endpoints basically work as expected for finals, so also the number of ends and arrows per end can be configured as expected.

## Internal changes for competition and match results

The ```IRuleService``` has been updated to implement two new endpoints ```SupportsMatch``` and ```SupportsCompetition```. This enables creating multiple rule services for the same match or competition types, for example for supporting knock-out finals only.

# Version 1.1.9

Date: 2025-02-15

## Re-structuring and additional documentation

Restructured the application so the services and calculations are no longer part of the ASP.net project.

Will enable creating a Windows Service later for quicker installation.

# Version 1.1.7

Date: 2025-02-10

## Updated endpoints (1)

The score-calculating endpoints have been updated to return partial resultes for every 10 ends in any match that takes more than 10 ends.

## New endpoints (2)

Endpoints have been added to the matches component to store UI-specific settings for a match. This is used to track and share the active results-tab over multiple displays.

## New endpoints (3)
Endpoints have been added for GET /match, GET /competition and GET /ruleset to only return items for a specific participant list. These endpoints are  ```GET /list/{listId}/match```, ```GET /list/{listId}/competition``` and ```GET /list/{listId}/ruleset```. We're using the currently selected participant list as the new aggregate root of the data model. All non-configuration data in the system always belongs to a participant-list.

## Increased token timeout
The token timeout duration has been increased to 150 minutes, preventing admins from having to log in again mid-match for most matches.

