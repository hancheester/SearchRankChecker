<div id="top"></div>

## Search Rank Checker
This application automates the process of tracking a website's search engine ranking for specified keywords, returning the positions where the URL appears in the top 100 search results, simplifying SEO monitoring for improved exposure.


## Table of Contents 
1. [Prerequisites](#prerequisites) 
2. [Setup](#setup)
3. [Design](#design) 
4. [Usage](#usage) 
5. [Testing](#testing) 
6. [Troubleshooting](#troubleshooting) 


## Prerequisites 
- Microsoft Visual Studio 2022
- SQL Express
- .NET SDK (version 8.0) 
- Internet connection (for search engine queries)


## Setup
1. Open the solution file (`.sln`) in Visual Studio.
2. Navigate to `SearchRankChecker.Api\appsettings.Development.json` to observe the connection string (`Server=localhost\\SQLEXPRESS;Database=SearchRankCheckerDb;Trusted_Connection=True;MultipleActiveResultSets=true`). `SearchRankChecker.Api` uses a database named `SearchRankCheckerDb`. 
3. Ensure 2 projects (`SearchRankChecker.Api` and `SearchRankChecker.Client`) are configured to start.
   ![image](https://github.com/user-attachments/assets/c906a65e-02c3-4a20-ba94-d0b27de42c87)
5. Press `F5` to run the solution.
6. The database `SearchRankCheckerDb` would be automatically created.
	- Optionally, database can be manually created by executing [`InitDb.sql`](https://github.com/hancheester/SearchRankChecker/blob/master/InitDb.sql) in SSMS (Microsoft SQL Server Management Studio). The file could be found within the root folder of the project.
	
 ```sql
-- Create the database
CREATE DATABASE SearchRankCheckerDb;
GO

-- Use the new database
USE SearchRankCheckerDb;
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
```

6. Two browser windows would appear during the run.
	- SPA (https://localhost:7094/)
	- API (https://localhost:7225/swagger) - for testing purpose

<p align="right"><a href="#top">back to top</a></p>

## Design
### API
The backend API is built using **ASP.NET Core 8.0** and **Entity Framework Core**, leveraging a **SQL Express** database. It has 3 important layers which are Presentation Layer (API Controllers), Infrastructure Layer and Application Layer. 

**Dependency Injection** is used throughout the API. Additionally, the **Mediator pattern** is employed for handling application requests and responses, ensuring loose coupling between components.

### SPA
The **SPA** is developed using **Blazor WebAssembly (WASM)**. The UI is built using the **MudBlazor** component library for a modern look and feel.

Communication between the SPA and the API is handled via an API client, which is also structured around the **Mediator pattern**.

State management is achieved using **Fluxor**, a library that provides a **Redux-like** store for Blazor applications.

<p align="right"><a href="#top">back to top</a></p>

## Usage
### Search

![image](https://github.com/user-attachments/assets/3fa92714-6637-4c1c-9215-f7f9e342424e)

Enter search term, targeted URL and intended search engine. Click a button `Go`, result would appear on the right side of the page to show all found page results with ranking number. Note that each result has a link which users can go and check the page.

### History

![image](https://github.com/user-attachments/assets/289f3438-9cdb-4ce3-92c8-4025c24e664c)

Start by clicking a button `Search` to show all previous search results. 
Each row has 3 action buttons. 

Button `Show` is to display page results with ranking number. 

Button `Repeat` is to go back to *Search* page and automatically perform the search again using the same search term, URL and search engine for better user experience. 

Button `Delete` is to delete the entry from database.

The search results table supports filtering, sorting and pagination.

<p align="right"><a href="#top">back to top</a></p>

## Testing
- Open the Test Explorer in Visual Studio:
    - Go to **Test** > **Test Explorer**.
- Run all tests:
    - Click on the **Run All** button.
- Verify that all tests pass successfully.

![image](https://github.com/user-attachments/assets/10a389f9-5eea-4dda-9f5b-633a806fb390)


<p align="right"><a href="#top">back to top</a></p>

## Troubleshooting
- Verify that the connection string in `SearchRankCheck.Api\appsettings.Development.json` is correctly configured. The connection string is `Server=localhost\\SQLEXPRESS;Database=SearchRankCheckerDb;Trusted_Connection=True;MultipleActiveResultSets=true`.
- Refer to log file(s) (`log-YYYMMDD.txt`) within the `SearchRankCheck.Api`'s project folder for more information.

![image](https://github.com/user-attachments/assets/acc2b818-6f25-4666-9fd2-743746e08a66)

<p align="right"><a href="#top">back to top</a></p>

## Contact
Han Chee - [@hancheester](https://x.com/hancheester) - hanchee@codecultivation.com

Project Link: https://github.com/hancheester/SearchRankChecker

<p align="right"><a href="#top">back to top</a></p>
