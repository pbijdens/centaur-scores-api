Build started...
Build succeeded.
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240303072601_InitialDatabase')
BEGIN
    CREATE TABLE `Matches` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MatchCode` longtext NOT NULL,
        `MatchName` longtext NOT NULL,
        `NumberOfEnds` int NOT NULL,
        `ArrowsPerEnd` int NOT NULL,
        `AutoProgressAfterEachArrow` tinyint(1) NOT NULL,
        `ScoreValuesJson` longtext NOT NULL,
        `GroupsJSON` longtext NOT NULL,
        `SubgroupsJSON` longtext NOT NULL,
        `TargetsJSON` longtext NOT NULL,
        `LijnenJSON` longtext NOT NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240303072601_InitialDatabase')
BEGIN
    CREATE TABLE `Settings` (
        `Name` varchar(255) NOT NULL,
        `JsonValue` longtext NULL,
        PRIMARY KEY (`Name`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240303072601_InitialDatabase')
BEGIN
    CREATE TABLE `Participants` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `MatchId` int NOT NULL,
        `DeviceID` longtext NOT NULL,
        `Lijn` longtext NOT NULL,
        `Name` longtext NOT NULL,
        `Group` longtext NOT NULL,
        `Subgroup` longtext NOT NULL,
        `Target` longtext NOT NULL,
        `Score` int NOT NULL,
        `EndsJSON` longtext NOT NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Participants_Matches_MatchId` FOREIGN KEY (`MatchId`) REFERENCES `Matches` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240303072601_InitialDatabase')
BEGIN
    CREATE INDEX `IX_Participants_MatchId` ON `Participants` (`MatchId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240303072601_InitialDatabase')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20240303072601_InitialDatabase', '8.0.2');
END;

COMMIT;

START TRANSACTION;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240930054054_ParticipantLists')
BEGIN
    ALTER TABLE `Participants` ADD `ParticipantListEntryId` int NULL;
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240930054054_ParticipantLists')
BEGIN
    CREATE TABLE `ParticipantLists` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` longtext NOT NULL,
        PRIMARY KEY (`Id`)
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240930054054_ParticipantLists')
BEGIN
    CREATE TABLE `ParticipantListEntries` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `ListId` int NOT NULL,
        `Name` longtext NOT NULL,
        `Group` longtext NOT NULL,
        `Subgroup` longtext NOT NULL,
        PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ParticipantListEntries_ParticipantLists_ListId` FOREIGN KEY (`ListId`) REFERENCES `ParticipantLists` (`Id`) ON DELETE CASCADE
    );
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240930054054_ParticipantLists')
BEGIN
    CREATE INDEX `IX_ParticipantListEntries_ListId` ON `ParticipantListEntries` (`ListId`);
END;

IF NOT EXISTS(SELECT * FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20240930054054_ParticipantLists')
BEGIN
    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20240930054054_ParticipantLists', '8.0.2');
END;

COMMIT;


