<div id="top"></div>

## Search Rank Checker
---
This application automates the process of tracking a website's search engine ranking for specified keywords, returning the positions where the URL appears in the top 100 search results, simplifying SEO monitoring for improved sales strategy.


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
4. Press `F5` to run the solution.
5. The database `SearchRankCheckerDb` would be automatically created.
	- Optionally, database can be manually created by executing `InitDb.sql` in SSMS (Microsoft SQL Server Management Studio). The file could be found within the root folder of the project.
6. 2 browser windows would appear during the run.
	- SPA (https://localhost:7094/)
	- API (https://localhost:7225/swagger) - for testing purpose

<p align="right">(<a href="#top">back to top</a>)</p>

## Design
### API
The backend API is built using **ASP.NET Core 8.0** and **Entity Framework Core**, leveraging a **SQL Express** database. It has 3 important layers which are Presentation Layer (API Controllers), Infrastructure Layer and Application Layer. 

**Dependency Injection** is used throughout the API. Additionally, the **Mediator pattern** is employed for handling application requests and responses, ensuring loose coupling between components.

### SPA
The **SPA** is developed using **Blazor WebAssembly (WASM)**. The UI is built using the **MudBlazor** component library for a modern look and feel.

Communication between the SPA and the API is handled via an API client, which is also structured around the **Mediator pattern**.

<p align="right">(<a href="#top">back to top</a>)</p>

## Usage
### Search
Enter search term, targeted URL and intended search engine. Click a button `Go`, result would appear on the right side of the page to show all found page results with ranking number.

### History
Start by clicking a button `Search` to show all previous search results. Each row has 3 action buttons. 

Button `Show` is to display page results with ranking number. 
Button `Repeat` is to go back to Search page and search again using the same search term, URL and search engine for better user experience. 
Button `Delete` to delete the row from database.

The search results table supports filtering, sorting and pagination.

<p align="right">(<a href="#top">back to top</a>)</p>

## Testing
- Open the Test Explorer in Visual Studio:
    - Go to **Test** > **Test Explorer**.
- Run all tests:
    - Click on the **Run All** button.
- Verify that all tests pass successfully.

<p align="right">(<a href="#top">back to top</a>)</p>

## Troubleshooting
- Verify that the connection string in `SearchRankCheck.Api\appsettings.Development.json` is correctly configured. The connection string is `Server=localhost\\SQLEXPRESS;Database=SearchRankCheckerDb;Trusted_Connection=True;MultipleActiveResultSets=true`.
- Refer to API log files (`log-YYYMMDD.txt`) within the project folder `SearchRankCheck.Api` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>

## Contact
Han Chee - [@hancheester](https://x.com/hancheester) - hanchee@codecultivation.com

Project Link: https://github.com/hancheester/SearchRankChecker

<p align="right">(<a href="#top">back to top</a>)</p>