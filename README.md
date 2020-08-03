# DativeBackend
Developed by Xabier Casado.

Requirements:
- **MySQL**:  Ver 8.0.21 for Linux on x86_64 (MySQL Community Server - GPL)
- **dotnet**: 3.1.302
- **dotnet-ef**: 3.1.6
- **Microsoft.EntityFrameworkCore.Design**: 3.1.6
- **MySql.Data.EntityFrameworkCore**: 8.0.21

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
