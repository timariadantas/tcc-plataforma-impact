#!/bin/bash
set -e  # Sai se algum comando falhar

# Rede do Docker Compose
NETWORK_NAME=plataforma-tcc_tcc-network

# Carrega variáveis do .env
export $(grep -v '^#' .env | xargs) 

# Função para rodar os testes de um serviço
run_tests() {
    SERVICE_NAME=$1
    SERVICE_PATH=$2
    TEST_PROJECT_RELATIVE_PATH=$3  # caminho relativo ao SERVICE_PATH

    echo "*******************************************"
    echo "Rodando testes do serviço $SERVICE_NAME..."
    

    docker run --rm \
        --network plataforma-tcc_tcc-network \
        -v "$PWD/$SERVICE_PATH":/src \
        -w /src \
        -e DOTNET_RUNNING_IN_CONTAINER=true \
        -e CLIENT_DB_HOST=${CLIENT_DB_HOST} \
        -e CLIENT_DB_PORT=${CLIENT_DB_PORT} \
        -e CLIENT_DB_USER=${CLIENT_DB_USER} \
        -e CLIENT_DB_PASSWORD=${CLIENT_DB_PASSWORD} \
        -e CLIENT_DB_NAME=${CLIENT_DB_NAME} \
        -e PRODUCT_DB_HOST=${PRODUCT_DB_HOST} \
        -e PRODUCT_DB_PORT=${PRODUCT_DB_PORT} \
        -e PRODUCT_DB_USER=${PRODUCT_DB_USER} \
        -e PRODUCT_DB_PASSWORD=${PRODUCT_DB_PASSWORD} \
        -e PRODUCT_DB_NAME=${PRODUCT_DB_NAME} \
        -e CART_DB_HOST=${CART_DB_HOST} \
        -e CART_DB_PORT=${CART_DB_PORT} \
        -e CART_DB_USER=${CART_DB_USER} \
        -e CART_DB_PASSWORD=${CART_DB_PASSWORD} \
        -e CART_DB_NAME=${CART_DB_NAME} \
        mcr.microsoft.com/dotnet/sdk:8.0 \
        dotnet test "$TEST_PROJECT_RELATIVE_PATH" --logger "console;verbosity=normal"
}

# ClientService
run_tests "ClientService" "services/client-service" "ClientService.Tests/ClientService.Tests.csproj"

# ProductService
run_tests "ProductService" "services/product-service" "ProductService.Tests/ProductService.Tests.csproj"

# CartService
run_tests "CartService" "services/cart-service" "CartService.Tests/CartService.Tests.csproj"

# CurrencyService
run_tests "CurrencyService" "services/currency-service" "CurrencyService.Tests/CurrencyService.Tests.csproj"

echo "**************************************************"
echo "Finalizando ... Todos os testes foram executados!"

