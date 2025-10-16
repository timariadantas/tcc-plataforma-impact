# 🚀 MEU TCC - Sistema de Serviços
**Plataforma Impact**

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)

---

Projeto desenvolvido como **TCC da Plataforma Impact**, simulando um ambiente real de empresa, com **serviços independentes**, **APIs REST**, **logs**, **testes** e **Docker**.  

---
## 📂 Estrutura do Projeto
 
```mermaid
flowchart TD
%% Estilo geral
classDef service fill:#f9f,stroke:#333,stroke-width:1px;
classDef db fill:#bbf,stroke:#333,stroke-width:1px;
classDef proxy fill:#fb8,stroke:#333,stroke-width:1px;
classDef test fill:#8f8,stroke:#333,stroke-width:1px;

%% Caddy como proxy
Caddy[("Caddy Reverse Proxy")]:::proxy

%% Serviços
Client[("Client Service")]:::service
Product[("Product Service")]:::service
Cart[("Cart Service")]:::service
Currency[("Currency Service (In-Memory API)")]:::service

%% Bancos de dados
ClientDB[("Client DB")]:::db
ProductDB[("Product DB")]:::db
CartDB[("Cart DB")]:::db

%% Testes
Tests[("Integration Tests (Docker)")]:::test

%% Conexões
Caddy --> Client
Caddy --> Product
Caddy --> Cart
Caddy --> Currency

Client --> ClientDB
Product --> ProductDB
Cart --> CartDB
Currency -.-> |"API em memória"| Currency

Tests --> Client
Tests --> Product
Tests --> Cart
Tests --> Currency


```




⚙️ Tecnologias Utilizadas

C# 11 / .NET 8
PostgreSQL
Docker & Docker Compose
ULID para identificação global
Logs estruturados 
Caddy

Esse projeto foi contruido com a ajuda do meu mentor Rafael Fino , Jedis da Plataforma Impact e outros colaboradores .


📝 Licença

MIT License © 2025 Maria Dantas
