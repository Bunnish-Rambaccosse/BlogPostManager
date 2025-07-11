# 🛠️ Blogger’s Hub – .NET Core Web API (Backend)
This is the backend of the Blogger’s Hub full-stack application, built with ASP.NET Core Web API. It provides secure endpoints for user authentication and blog post management, and serves as the API layer for the React frontend.

## 🚀 Tech Stack
- ASP.NET Core Web API
- Entity Framework Core (with Code-First Migrations)
- ASP.NET Core Identity (for user auth)
- SQL Server / SQLite (configurable DB)
- JWT (JSON Web Tokens) for Authentication
- CORS enabled for cross-origin frontend access

## 📦 Features
- JWT-based Authentication
- User Registration & Login
- Create / Edit / Delete Posts
- Public Post Access (No login required)
- Tag
- Built-in Validation & Error Handling
- Logging

## ⚙️ Getting Started

### Clone the Repository 

``` bash
git clone https://github.com/Bunnish-Rambaccosse/BlogPostManager.git
```

### Set Up the Database

#### Update the appsettings.json for AuthAPI

- Update your appsettings.json with your connection string:
  
  ``` Json
  "ConnectionStrings": {
  "DefaultConnection": "Server=(yourservername);Database=BlogPostManager_Auth;Trusted_Connection=True;TrustServerCertificate=True"}
  ```
#### Update the appsettings.json for PostAPI

- Update your appsettings.json with your connection string:
  
  ``` Json
  "ConnectionStrings": {
  "DefaultConnection": "Server=(yourservername);Database=BlogPostManager_Post;Trusted_Connection=True;TrustServerCertificate=True"}
  ```

### Run Migrations

1. Open Package Manager Console on Visual Studio.

2. Select AuthAPI under Default Project and run the below migration script.

    ``` bash
    update-database
    ```

3. Select PostAPI under Default Project and run the below migration script.

    ``` bash
    update-database
    ```

### CORS Setup

Ensure CORS is configured to allow requests from your frontend.

#### Update Program.cs

Update the origins for both AuthAPI and PostAPI in program.cs with your frontend url.

``` csharp

    builder.WithOrigins("http://localhost:3000")
       .AllowAnyHeader()
       .AllowAnyMethod()
       ;
```

### Run APIs

1. Right click on BlogPostManager solution and select *Configure StartUp Projects*

2. Select *Multiple startup projects*

3. Under Action column, select *start* for AuthAPI and PostAPI and click *Apply*

4. Click Start to run both APIs.
