# 🔨 Stage 1 - Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiando arquivos .csproj para restaurar dependências
COPY *.sln .
COPY building-blocks/EcommerceModular.Application/*.csproj ./building-blocks/EcommerceModular.Application/
COPY building-blocks/EcommerceModular.Domain/*.csproj ./building-blocks/EcommerceModular.Domain/
COPY building-blocks/EcommerceModular.Infrastructure/*.csproj ./building-blocks/EcommerceModular.Infrastructure/
COPY src/services/Orders/Orders.API/*.csproj ./src/services/Orders/Orders.API/
COPY src/services/Orders/Orders.KafkaConsumer/*.csproj ./src/services/Orders/Orders.KafkaConsumer/
COPY src/services/Orders/Orders.Consumer/*.csproj ./src/services/Orders/Orders.Consumer/
COPY tests/Orders.Application.Tests/*.csproj ./tests/Orders.Application.Tests/

# Restaurando pacotes NuGet
RUN dotnet restore

# Copiando todo o restante do código
COPY . .

EXPOSE 5100

# Setando o diretório de trabalho para Orders.API
WORKDIR /app/src/services/Orders/Orders.API

# Publicando a aplicação em modo Release
RUN dotnet publish -c Release -o out

# 🚀 Stage 2 - Imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiando arquivos publicados do stage anterior
COPY --from=build /app/src/services/Orders/Orders.API/out ./

# Definindo o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "Orders.API.dll"]