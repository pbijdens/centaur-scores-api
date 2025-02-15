# TotalScoreBasedLogic module

A not very clear explanation that basically sumamrizes how this module works.

## Context

The CentaurScores backend uses rulesets to create pre-defined formats for competitions and matches.

In this context, a competition is a sequence of compatible matches; for example a sequence of six 18m matches
over 30 arrows and a sequence of six 25m matches over 30 arrows using standard scoring rules could make up
a competition over twelve matches.

In this context, a ruleset would be "18m 30 arrows" and "25m 30 arrows". Each ruleset in the system is 
identified by a unique name. The ruleset group would be "Indoor 30arrows at 18 or 25"

The logic behind rulesets is implemented in so-called rules ervices. These implement the IRuleService
that requires the following interface functions:
1. GetSupportedRulesets() to return the names of the rulesets for which this service may be used.
1. CalculateSingleMatchResult() is used to take one single match out of a competition, and calculate the scores. This also powers the live scoring system.
1. CalculateCompetitionResult() is used to calculate the results for a full competition consisting of one or many separate matches.

As an example, AHV centaur organizes a lancaster-scored round as part of the internal competition. This consists of three sets of three matches: a first 30-arrow lancaster round, a second 30-arrow lancaster round and a knock-out final. The individual match scores for all the 30-arrow matches can be calculated using the ```TotalScoreBasedLogic``` module. The match scores for the final rounds can be calculated using the FinalRoundResultCalculator. 

However, the competition score for the full competition does not add-up the scores. Instead, archers score points in this competition based on their achievements in the individual matches: Archers are sorted based on their final position, meaning that the finalists are put at the top of the list, in order of their position in the finals, followed by everyone who did not make it to the the finals, ordered by their qualification scores. Then points are assigned 12, 10, 8, 7, 6, 5, 4, 3, 2, 1*

## About this module

This module offers a default implementation for the match logic and competition logic. It offers a base class ```TotalScoreBasedResultCalculatorBase``` that can be parameterized for all types of competition where for a match simply the total score is used to order the results, and where at competition level the total score (possibly abandoning the *n* lowest scores) for each of the match-types is determined and the total score over all matches is used to calculate a total.

## Usage

### GetSupportedRulesets
```GetSupportedRulesets()``` is abstract. Each competition module should internally decide which types of match it supports.

### CalculateCompetitionResultForDB

```CalculateCompetitionResultForDB``` implements all logic for the CalculateCompetitionResult() method. Classes deriving from this class should implement their own CalculateCompetitionResult() but it can be as simple as this:

```C#
public async Task<CompetitionResultModel> CalculateCompetitionResult(int competitionId)
{
    using CentaurScoresDbContext db = new(configuration);
    var result = await CalculateCompetitionResultForDB(db, competitionId);
    return result;
}
```

### CalculateSingleMatchResultFromDB

`This method implements all logic for the CalculateSingleMatchResult() method. Also here, classes deriving from this base class can get away with implementing only a wrapper that adds a database reference:


```C#
public async Task<MatchResultModel> CalculateSingleMatchResult(int matchId)
{
    using CentaurScoresDbContext db = new(configuration);
    var result = await CalculateSingleMatchResultForDB(db, matchId);
    return result;
}
```

## The inner workings

## Competition results

To calculate the competition results first for each match in the competition the results are calculated.

Next, these results are grouped by the type of match that they represent. So, for a competition with 18m3p and 25m1p matches we end up with a number of scores for each archer for each of those types of matches.

If there are more than 5 (...) scores for a specific type of match, the lowest scores of those are discareded until at most 5 remain.

These are then simply added up, and then the totals for all match types is added up also, providing us with the competition score per archer.

We sort those per discipline or discipline and class, yielding a match result.

Archers are ideally linked to the members list. That makes it easy to track archers over multiple matches (free entry of names).

## Single match results

To determine score for one single match, the following steps are taken:

1. The list of participants and their arrow-by-arrow score is read from the database. Small inconistencies in input like missing ends or badly calculated end-totals are fixed before continuing.
1. The system builds the dat lists to aggregate on later, so discipline, age group, etc.
1. For every participant, the tie-breaker information is populated. This basically is the number of 10s, 9s, 8s, etc. Calculating this is computationally expensive, so we calculate that once so we can quickly access it during comparison when we potentially need it very often.
1. Since we can't re-use any previously calculated results (because of tie-breakers), we invoke ```SortSingleMatchResult``` three times:
    1. First on the ungrouped participant list
    1. Next on the participant list per discipline
    1. Finally on the participant list per age group per discipline
  
### SortSingleMatchResult

This is where the 'magic' happens

1. First for each participant the current per-arrow average is calculated, as well as the number of arrows that this participant shot.
1. Next for each participant their personal best score is obtained from the database
1. Next the (partial) list of match results is sorted using the tiebreakingComparer. Any tie-breaking data used is added to the result.

If two results are identical even using the tiebreaker, they are marked as such.

## On competitions with final rounds

If a competition uses final rounds, then the scoring for those competitions is going to be different. There is a separate competition result calculator for those.