# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código e compila
COPY . ./
RUN dotnet publish -c Release -o /app

# Etapa 2: imagem final para rodar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Expor porta padrão
EXPOSE 80

# Comando para rodar
ENTRYPOINT ["dotnet", "ClientService.dll"]
