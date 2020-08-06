# DativeBackend
Developed by Xabier Casado.

Doc generated with Swashbuckle and available at: https://localhost:5001/

Requirements:
- **dotnet**: 3.1.302
- **MySQL**:  Ver 8.0.21 for Linux on x86_64 (MySQL Community Server - GPL)

Packages:
- **MySql.Data.EntityFrameworkCore**: 8.0.21
- **Microsoft.EntityFrameworkCore.Design**: 3.1.6
- **Microsoft.VisualStudio.Web.CodeGeneration.Design**: 3.1.3
- **Microsoft.EntityFrameworkCore.SqlServer**: 3.1.6
- **Microsoft.AspNetCore.Authentication.JwtBearer**: 3.1.6
- **Microsoft.Extensions.Caching.StackExchangeRedis**: 3.1.6
- **Confluent.Kafka**: 1.5.0
- **Swashbuckle.AspNetCore**: 5.5.1

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

## Run Redis docker
```
docker pull redis

docker run --name container-redis -p 6379:6379 -d redis

docker exec -it container-redis sh

redis-cli
```

## Run Kafka docker
```
docker pull bitnami/kafka:latest

docker network create app-tier --driver bridge

docker run -d --name container-zookeeper \
    --network app-tier \
    -e ALLOW_ANONYMOUS_LOGIN=yes \
    bitnami/zookeeper:latest

docker run -d --name container-kafka \
    --network app-tier \
    -e ALLOW_PLAINTEXT_LISTENER=yes \
    -e KAFKA_CFG_ZOOKEEPER_CONNECT=container-zookeeper:2181 \
    bitnami/kafka:latest

docker run -it --rm \
    --network app-tier \
    -e KAFKA_CFG_ZOOKEEPER_CONNECT=container-zookeeper:2181 \
    bitnami/kafka:latest kafka-topics.sh --list  --zookeeper container-zookeeper:2181
```
