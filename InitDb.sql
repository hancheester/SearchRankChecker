-- Create the database
CREATE DATABASE SearchRankCheckerDb3;
GO

-- Use the new database
USE SearchRankCheckerDb3;
GO

-- Create the SearchHistory table
CREATE TABLE SearchHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SearchTerm NVARCHAR(MAX),
    TargetUrl NVARCHAR(MAX),
    SearchDate DATETIME2 NOT NULL,
    SearchEngine NVARCHAR(MAX)
);
GO

-- Create the SearchResultEntry table
CREATE TABLE SearchResultEntry (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SearchHistoryId INT NOT NULL,
    Url NVARCHAR(MAX),
    Rank INT NOT NULL,
    CONSTRAINT FK_SearchResultEntry_SearchHistory FOREIGN KEY (SearchHistoryId)
        REFERENCES SearchHistory(Id)
        ON DELETE CASCADE
);
GO