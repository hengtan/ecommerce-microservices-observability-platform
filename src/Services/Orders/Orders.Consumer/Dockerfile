# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.sln .
COPY building-blocks/EcommerceModular.Application/*.csproj ./building-blocks/EcommerceModular.Application/
COPY building-blocks/EcommerceModular.Domain/*.csproj ./building-blocks/EcommerceModular.Domain/
COPY building-blocks/EcommerceModular.Infrastructure/*.csproj ./building-blocks/EcommerceModular.Infrastructure/
COPY src/services/Orders/Orders.API/*.csproj ./src/services/Orders/Orders.API/
COPY src/services/Orders/Orders.KafkaConsumer/*.csproj ./src/services/Orders/Orders.KafkaConsumer/
COPY tests/Orders.Application.Tests/*.csproj ./tests/Orders.Application.Tests/
COPY src/services/Orders/Orders.Consumer/*.csproj ./src/services/Orders/Orders.Consumer/

RUN dotnet restore

COPY . .
WORKDIR /app/src/services/Orders/Orders.Consumer
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/services/Orders/Orders.Consumer/out ./
ENTRYPOINT ["dotnet", "Orders.Consumer.dll"]