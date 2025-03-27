
# EcommerceModular 🛒 - Microserviços com .NET 9, DDD, CQRS e Observabilidade Completa

Projeto moderno de e-commerce baseado em arquitetura de microserviços com foco em escalabilidade, performance, mensageria, testes e observabilidade completa.

---

## 🚀 Tecnologias e Arquitetura

### 🧱 Estrutura do Projeto
- 🔹 Arquitetura **DDD + Clean Architecture**
- 🔹 Separação por Domínio: `Pedidos`, `Produtos`, `Pagamentos`, `Notificações`
- 🔹 Padrões: **CQRS + Event Sourcing**
- 🔹 Comunicação entre microserviços com **gRPC**
- 🔹 Camada de cache com **Redis**
- 🔹 Comunicação assíncrona via **Apache Kafka**
- 🔹 Autenticação via **JWT** (estrutura preparada para migração futura para Identity Server)

### 🗃️ Bancos de Dados
- ✅ **PostgreSQL** para escrita (com Event Sourcing)
- ✅ **MongoDB** para leitura (projeção dos eventos)

### 🔍 Observabilidade
- ✅ Logs estruturados com **Serilog**
- ✅ Monitoramento de logs com **Elastic Stack (Elasticsearch, Logstash, Kibana)**
- ✅ Métricas expostas via **Prometheus**
- ✅ Dashboards com **Grafana**
- ✅ Health Checks (Liveness & Readiness)

### ✅ Testes & Qualidade
- ✅ Testes de unidade com **NUnit + Bogus**
- ✅ Cobertura de código **100% com Coverlet**
- ✅ Relatórios visuais com **ReportGenerator**
- ✅ Execução automática de testes com **GitHub Actions**
- ✅ Validações com **FluentValidation**
- ✅ Documentação automática com **Swagger + ReDoc**

### 🐳 Infraestrutura
- ✅ Ambiente isolado com **Docker + Docker Compose**
- ✅ Configuração futura preparada para **Kubernetes**
- ✅ CI/CD com **GitHub Actions Free Tier**:
  - Build automático a cada push no `main`
  - Execução dos testes
  - Geração das imagens Docker
  - Validação de cobertura de testes

---

## 📂 Estrutura Inicial
```
EcommerceModular/
├── src/
│   ├── Pedidos/
│   │   ├── Pedidos.API/
│   │   ├── Pedidos.Application/
│   │   ├── Pedidos.Domain/
│   │   ├── Pedidos.Infrastructure/
│   │   └── Pedidos.EventSourcing/
│   └── ...
├── tests/
│   └── Pedidos.Tests/
├── docker/
│   ├── kafka/
│   ├── mongodb/
│   ├── postgres/
│   ├── redis/
│   ├── elasticstack/
│   ├── prometheus/
│   └── grafana/
├── docker-compose.yml
├── .github/workflows/ci.yml
└── README.md
```

---

## 📈 Fluxo do Sistema (resumo visual)

1. Usuário envia um pedido via `POST /api/pedidos`
2. Pedido é validado (FluentValidation) e autenticado (JWT)
3. Dados são gravados no **PostgreSQL** como eventos
4. Eventos são enviados para o **Kafka**
5. Um **consumer** projeta os eventos para o **MongoDB**
6. Outro serviço pode consumir esse evento via Kafka
7. **Redis** é atualizado com cache para leitura rápida
8. **Elastic** armazena os logs estruturados
9. **Prometheus** coleta métricas
10. **Grafana** e **Kibana** mostram tudo em dashboards

---

## 📚 Aprendizados & Boas Práticas

- SOLID + Clean Code aplicado
- Separação clara de responsabilidades
- Totalmente extensível para novos serviços
- Arquitetura pronta para adicionar:
  - ✅ Temporal.io (futuramente)
  - ✅ Identity Server
  - ✅ Deploy com Kubernetes (Helm, ArgoCD, etc.)

---

## 🧪 Para rodar localmente
```bash
docker compose up --build
```

---

## 🛠️ Melhorias Futuras
- 🔁 **Orquestração com Temporal.io**
- ☁️ **Deploy real com Kubernetes + GitHub Actions**
- 🌐 **Front-end com React + Tailwind**
- ⚙️ **Painel Admin com dashboards + alertas**
- 🔒 **Migração para Identity Server (autenticação avançada)**

---

Feito com ❤️ por Isabela
