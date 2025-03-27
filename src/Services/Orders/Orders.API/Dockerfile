# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy sln and csproj
COPY *.sln .
COPY building-blocks/**/*.csproj ./building-blocks/
COPY src/Services/Orders/Orders.API/*.csproj ./src/Services/Orders/Orders.API/

RUN dotnet restore

# Copy the rest and publish
COPY . .
WORKDIR /app/src/Services/Orders/Orders.API
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/src/Services/Orders/Orders.API/out ./
ENTRYPOINT ["dotnet", "Orders.API.dll"]