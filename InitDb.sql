USE [master]
GO

CREATE DATABASE [SearchRankCheckerDb]
GO

USE [SearchRankCheckerDb]
GO

CREATE TABLE [dbo].[SearchHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SearchTerm] [nvarchar](max) NULL,
	[TargetUrl] [nvarchar](max) NULL,
	[SearchDate] [datetime2](7) NOT NULL,
	[SearchEngine] [nvarchar](max) NULL,
 CONSTRAINT [PK_SearchHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[SearchResultEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SearchHistoryId] [int] NOT NULL,
	[Url] [nvarchar](max) NULL,
	[Rank] [int] NOT NULL,
 CONSTRAINT [PK_SearchResultEntry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SearchResultEntry]  WITH CHECK ADD  CONSTRAINT [FK_SearchResultEntry_SearchHistory] FOREIGN KEY([SearchHistoryId])
REFERENCES [dbo].[SearchHistory] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SearchResultEntry] CHECK CONSTRAINT [FK_SearchResultEntry_SearchHistory]
GO