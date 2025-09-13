# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura dependências
COPY ProductService.csproj ./
RUN dotnet restore ProductService.csproj

# Copia todo o restante do código
COPY . ./

# Publica o projeto
RUN dotnet publish ProductService.csproj -c Release -o /app/publish

# Etapa runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.dll"]
