# 🚀 TCC - Sistema de Serviços
**Plataforma Impact**

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Docker](https://img.shields.io/badge/Docker-Compose-blue)
![Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)

---

Projeto desenvolvido como **TCC da Plataforma Impact**, simulando um ambiente real de empresa, com **serviços independentes**, **APIs REST**, **logs**, **testes** e **Docker**.  

---

## 📂 Estrutura do Projeto

```text
ClientService/
├── .env
├── .gitignore
├── docker-compose.yml
├── docker-initdb/
│   └── init.sql
├── services/
│   └── client-service/
│       ├── ClientService.csproj
│       ├── Program.cs
│       ├── Dockerfile
│       ├── Logging/
│       │   └── TabLogger.cs
│       ├── Service/
│       │   └── ClientService.cs
│       └── Storage/
│           └── ClientStorage.cs
└── volumes/
    └── postgres_data/


---

## ⚙️ Tecnologias Utilizadas

- **C# 11 / .NET 8**
- **PostgreSQL**
- **Docker & Docker Compose**
- **ULID** para identificação global
- **Logs estruturados** com medição de tempo de requisição


📝 Licença

MIT License © 2025 Maria Dantas
