# ASP.NET Core Identity & Book Management Application

## Project Overview by Diaz
This project is a web application built using **ASP.NET Core Razor Pages** with **ASP.NET Core Identity** for authentication and authorization.  
The application implements **User Management** and **Book Management (CRUD)** features as required in the assignment document.


## Tech Stack
- **Framework**: ASP.NET Core 8.0
- **UI**: Razor Pages
- **Authentication**: ASP.NET Core Identity
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Language**: C#
- **IDE**: Visual Studio

## How To Run 
## 1. Database & Migration
- Entity Framework Core migrations are used
- Tables include:
  - `AspNetUsers`
  - `AspNetRoles`
  - `Books`
- Database updates are applied using command:
 - `Add-Migration`
 - `InitialCreate`
Update-Database

## 2. Install Nuget Requirement On Project
- After Install, rebuild and clean then run again