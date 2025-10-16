# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o projeto principal
COPY ClientService/ClientService.csproj ClientService/
# Copia o projeto de testes
COPY ClientService.Tests/ClientService.Tests.csproj ClientService.Tests/
RUN dotnet restore ClientService/ClientService.csproj
RUN dotnet restore ClientService.Tests/ClientService.Tests.csproj

# Copia todo o código
COPY . .

# Publica a aplicação (para o container principal)
RUN dotnet publish ClientService/ClientService.csproj -c Release -o /app/publish

# Etapa runtime (apenas pra rodar a API normalmente)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClientService.dll"]
