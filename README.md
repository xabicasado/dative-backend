# DativeBackend
Developed by Xabier Casado.

Requirements:
- **dotnet**: 3.1.302
- **MySQL**:  Ver 8.0.21 for Linux on x86_64 (MySQL Community Server - GPL)

Packages:
- **MySql.Data.EntityFrameworkCore**: 8.0.21
- **Microsoft.EntityFrameworkCore.Design**: 3.1.6
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: 3.1.3
- **Microsoft.EntityFrameworkCore.SqlServer**: 3.1.6

Tools:
- **dotnet-ef**: 3.1.6
- **dotnet-aspnet-codegenerator**: 3.1.3

## Project creation
```
dotnet new webapi -o DativeBackend

dotnet add package MySql.Data.EntityFrameworkCore --version 8.0.21
```

## Entity Framework initialize
https://docs.microsoft.com/es-es/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

```
dotnet new tool-manifest

dotnet tool install dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet ef migrations add InitialCreate

dotnet ef database update
```
