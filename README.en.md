
<p align="right">
🇧🇷 <a href="README.md">Leia em Português</a>
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

> A modern, modular, and scalable e-commerce architecture built on .NET with observability and best practices in mind.

---

## 📦 Overview

**EcommerceModular** is a microservices-based e-commerce platform focused on:

- High scalability and fault-tolerance
- Out-of-the-box observability
- Clean architecture principles: DDD, CQRS, Event Sourcing
- Async communication using **Kafka**
- Efficient read projections with **MongoDB**
- Caching with **Redis**
- Durable persistence with **PostgreSQL**
- Complete unit and integration test coverage
- Fully containerized with Docker

---

## 🧱 Architecture

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

## 🧪 Testing

- ✅ Framework: **NUnit**
- ✅ Fake data generation: **Bogus**
- ✅ 100% code coverage via **Coverlet**
- ✅ Visual reports with **ReportGenerator**
- ✅ Retry & circuit breaker policies with **Polly**

---

## 📈 Observability

- 🟦 Structured logging with **Serilog**
- 📊 Metrics collection with **Prometheus** (coming soon)
- 📈 Dashboards via **Grafana** (coming soon)
- 🔎 Full log search via **Elastic Stack** (planned)
- 🛰️ Distributed tracing with **OpenTelemetry + Jaeger** (planned)

---

## 🧩 Modules and Services

### 🧾 `Orders.API`
- Create orders with `POST /api/orders`
- Get orders by ID via `GET /api/orders/{id}`
- Total calculated using Strategy Pattern based on customer type
- Read access with fallback: Redis → MongoDB (**cache-aside**)

### 📥 `Orders.Consumer`
- Kafka Consumer using `BackgroundService`
- Listens to topic `orders.created`
- Saves projections into **MongoDB**
- Can populate Redis when needed

### 💾 `RedisOrderReadProjection`
- Decorates read projection layer to use Redis cache first
- Automatically updates cache from MongoDB on cache miss

---

## 🧰 Tech Stack

- ✅ **.NET 9.0**
- ✅ **ASP.NET Core Web API**
- ✅ **Kafka** (via Bitnami)
- ✅ **MongoDB** (read model)
- ✅ **Redis** (cache)
- ✅ **PostgreSQL** (write model)
- ✅ **Docker + Docker Compose**
- ✅ **Serilog** for logging
- ✅ **GitHub Actions** for CI/CD

---

## 🗂️ Project Structure

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

## 🚀 Getting Started

1. Clone the repository:

```bash
git clone https://github.com/your-username/ecommerce-microservices-observability-platform.git
cd ecommerce-microservices-observability-platform
```

2. Launch services with Docker Compose:

```bash
docker-compose up -d
```

3. Run the API:

```bash
dotnet run --project services/Orders/Orders.API/Orders.API.csproj
```

---

## ✅ Roadmap

- [ ] Add `Products`, `Payments`, and `Notifications` services
- [ ] Add tracing support with OpenTelemetry + Jaeger
- [ ] Integrate Prometheus and Grafana dashboards
- [ ] Enable Elastic Stack (Kibana + Elasticsearch)
- [ ] Front-end in **React + Tailwind**
- [ ] Production-ready deployment with **Kubernetes**
- [ ] Full CI/CD automation

---

## 👩‍💻 Developed by

**hengtan** – A passionate .NET developer focused on clean code, scalable architecture, and real observability.  
💻🚀 *Let’s build systems that scale and shine.*

---

## 🌟 Contribute

If you like this project, star it ⭐, fork it 🍴, and contribute! PRs welcome 🙌

---

## 📜 License

This project is licensed under the MIT License.
