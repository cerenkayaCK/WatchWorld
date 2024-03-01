# WatchWorld
A sample N-layered .Net Core Project Demonstrating Clean Architecture and the Generic Repository Pattern

## Packages 

### ApplicationCore
- Ardalis.Specification

### Infastructure
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Design 
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Ardalis.Specification.EntityFrameworkCore

### Web
 
- Microsoft.EntityFrameworkCore.Design 

### UnitTests

## Migrations
Before running the following commands,ensure that "Web is set as the startup project . Run the following commands in the "Infrastructure" project.

### Infrastructure
```
Add-Migration InitialCreate -Context WatchWorldContext -OutputDir Data/Migrations
Update-Database -Context WatchWorldContext
```
```
Add-Migration InitialCreate -Context AppIdentityDbContext -OutputDir Identity/Migrations
Update-Database -Context WatchWorldContext
```
