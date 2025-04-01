<p align="right">
🌐 <a href="README.en.md">View in English</a>
</p>

<p align="right">
🇧🇷 <a href="README.md">Ver em Português</a>
</p>

# 🛒 EcommerceModular - Microservices Observability Platform

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet?logo=dotnet)
![Kafka](https://img.shields.io/badge/Kafka-Enabled-black?logo=apachekafka)
![MongoDB](https://img.shields.io/badge/MongoDB-Read%20Model-green?logo=mongodb)
![Redis](https://img.shields.io/badge/Redis-Cache-red?logo=redis)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Write%20Model-blue?logo=postgresql)
![CI/CD](https://img.shields.io/badge/GitHub%20Actions-Enabled-blue?logo=githubactions)
![Test Coverage](https://img.shields.io/badge/Test%20Coverage-100%25-success?logo=testinglibrary)
![Status](https://img.shields.io/badge/Status-In%20Progress-yellow)

> Uma arquitetura moderna, modular e escalável para e-commerce usando o que há de mais atual em tecnologia .NET!

---

## 📦 Visão Geral

A **EcommerceModular** é uma plataforma de e-commerce baseada em **microserviços** com foco em:

- Alta escalabilidade
- Observabilidade desde o dia 1
- Boas práticas arquiteturais (DDD, Clean Architecture, CQRS)
- Comunicação assíncrona com **Kafka**
- Projeções eficientes com **MongoDB**
- Cache otimizado com **Redis**
- Persistência com **PostgreSQL**
- Testes e cobertura completos
- Docker-ready para produção

---

## 🧱 Arquitetura

```plaintext
                         +------------------------+
                         |   API Gateway (WIP)    |
                         +----------+-------------+
                                    |
                +-------------------+---------------------+
                |                                         |
         +------+-------+                        +--------+-------+
         |  Orders.API   |                        | Orders.Consumer |
         +------+--------+                        +--------+--------+
                |                                             |
      +---------+----------+                       +----------+---------+
      | PostgreSQL (Write) |                       | Kafka Order Events |
      +--------------------+                       +----------+---------+
                                                               |
                                                +--------------+-------------+
                                                | MongoDB (Read Projection) |
                                                | Redis (Cache-aside)       |
                                                +---------------------------+
```

---

## 🧪 Testes

- ✅ Framework: **NUnit**
- ✅ Geração de dados fake: **Bogus**
- ✅ Cobertura de testes: **100%** via **Coverlet**
- ✅ Relatório visual: **ReportGenerator**
- ✅ Retry e circuit breaker com **Polly**

---

## 📈 Observabilidade

- 📦 **Serilog** para logs estruturados (console + arquivos)
- 📊 **Prometheus** para coleta de métricas (em breve)
- 📺 **Grafana** com dashboards técnicos (em breve)
- 🔎 **Elastic Stack (Elasticsearch + Kibana)** para logs detalhados (planejado)
- 📡 **OpenTelemetry + Jaeger** para rastreamento distribuído (planejado)

---

## 🧩 Módulos e Serviços

### 🧾 `Orders.API`
- Criação de pedidos via `POST /api/orders`
- Consulta por ID via `GET /api/orders/{id}`
- Cálculo do total com Strategy Pattern baseado no tipo de cliente
- Leitura com fallback: Redis → MongoDB (padrão **cache-aside**)

### 📥 `Orders.Consumer`
- Kafka Consumer assíncrono (`BackgroundService`)
- Escuta do tópico `orders.created`
- Projeção dos dados em **MongoDB**
- Integração com Redis (quando aplicável)

### 💾 `RedisOrderReadProjection`
- Decora leitura da projeção para utilizar Redis como cache primário
- Fallback em MongoDB com atualização automática do cache

---

## 🧰 Tecnologias e Ferramentas

- ✅ **.NET 9.0**
- ✅ **ASP.NET Core Web API**
- ✅ **Kafka** (via Bitnami no Docker)
- ✅ **MongoDB** (read model)
- ✅ **Redis** (cache)
- ✅ **PostgreSQL** (write model)
- ✅ **Docker + Docker Compose**
- ✅ **Serilog** para logs estruturados
- ✅ **CI/CD** com GitHub Actions (pipeline configurado)
- ✅ **Health Checks**, **Swagger**, e muito mais...

---

## 🗂️ Estrutura do Projeto

```
EcommerceModular/
├── building-blocks/
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   └── Shared/
├── services/
│   └── Orders/
│       ├── Orders.API/
│       └── Orders.Consumer/
├── docker/
│   ├── docker-compose.yml
│   ├── kafka/
│   ├── mongo/
│   ├── postgres/
│   └── redis/
├── tests/
│   └── Orders.Tests/
└── .github/
    └── workflows/
        └── ci.yml
```

---

## 🚀 Como rodar localmente

1. Clone o repositório:

```bash
git clone https://github.com/seu-user/ecommerce-microservices-observability-platform.git
cd ecommerce-microservices-observability-platform
```

2. Suba os containers com Docker Compose:

```bash
docker-compose up -d
```

3. Execute a API:

```bash
dotnet run --project services/Orders/Orders.API/Orders.API.csproj
```

---

## ✅ Roadmap (Próximos Passos)

- [ ] Criar serviços `Products`, `Payments` e `Notifications`
- [ ] Adicionar rastreamento com OpenTelemetry + Jaeger
- [ ] Integrar com Grafana e Prometheus
- [ ] Incluir Elastic Stack (logs ricos)
- [ ] Criar front-end em **React + Tailwind**
- [ ] Preparar deploy real com **Kubernetes**
- [ ] Pipeline completo de CI/CD

---

## 👩‍💻 Desenvolvido por

**hengtan** – Desenvolvedora .NET apaixonada por arquitetura limpa, sistemas resilientes e observabilidade real.  
💻🚀 *Let's build systems that scale and shine.*

---

## 🌟 Contribua!

Se esse projeto te inspirou ou te ajudou, ⭐ marque com uma estrela, fork e contribua!

---

## 📜 Licença

Este projeto está sob a licença MIT.
