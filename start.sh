#!/bin/bash

echo "Parando e removendo containers antigos..."
docker-compose down -v

echo "Construindo e subindo os containers..."
docker-compose up -d --build

echo "Listando containers em execução..."
docker ps

echo "A aplicação está pronta! Acesse no navegador: http://localhost:8000/"
