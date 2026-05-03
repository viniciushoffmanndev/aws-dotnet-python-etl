# 💼 Financial Event-Driven ETL Pipeline

### .NET • Python • AWS (SQS) • PostgreSQL • Docker

![Pipeline de Arquitetura](./images/pipeline-arquitetura.jpg)

<p align="center">
  <img src="https://via.placeholder.com/900x250" alt="event-driven pipeline banner"/>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8-blue">
  <img src="https://img.shields.io/badge/Python-3.11-yellow">
  <img src="https://img.shields.io/badge/PostgreSQL-15-blue">
  <img src="https://img.shields.io/badge/AWS-SQS-orange">
  <img src="https://img.shields.io/badge/Architecture-Event--Driven-green">
  <img src="https://img.shields.io/badge/Status-Production--Inspired-success">
</p>

---

## 🧠 Sobre o Projeto

Este projeto simula um **pipeline de processamento financeiro orientado a eventos**, inspirado em arquiteturas utilizadas em ambientes reais de produção.

A solução foi projetada para demonstrar como sistemas modernos lidam com:

* Processamento assíncrono
* Baixo acoplamento entre serviços
* Escalabilidade horizontal
* Resiliência a falhas

---

## 🎯 Problema de Engenharia

Sistemas monolíticos e síncronos apresentam limitações como:

* Gargalos de performance
* Alto acoplamento
* Falhas em cascata

👉 Este projeto resolve isso utilizando uma arquitetura baseada em **mensageria e eventos**, separando responsabilidades entre serviços independentes.

---

## 🏗️ Arquitetura da Solução

```text
Client → .NET API → PostgreSQL
              ↓
         SQS (LocalStack)
              ↓
        Python Worker
              ↓
         PostgreSQL (Status Update)
```

### 🔄 Fluxo de Processamento

1. API recebe uma transação financeira
2. Persiste os dados no PostgreSQL
3. Publica um evento na fila (SQS)
4. Worker Python consome a mensagem
5. Atualiza o status da transação no banco

---

## ⚙️ Stack Tecnológica

### 🔹 Backend (Producer)

* .NET 8 / ASP.NET Core (Minimal APIs)
* Entity Framework Core
* PostgreSQL (Npgsql)
* AWS SDK (SQS)

---

### 🔹 Worker (Consumer)

* Python 3.11+
* boto3
* psycopg (v3)

---

### 🔹 Infraestrutura

* Docker & Docker Compose
* PostgreSQL 15
* LocalStack (simulação AWS)

---

## 🚀 Como Executar

### 📦 Subir ambiente

```bash
docker compose up -d
```

---

### ▶️ Executar API (.NET)

```bash
cd src/producer-dotnet
dotnet run
```

---

### 🐍 Executar Worker (Python)

```bash
cd src/worker-python

# Windows
.\venv\Scripts\Activate

# Linux/Mac
source venv/bin/activate

python worker.py
```

---

### 📤 Testar envio de transação

```bash
Invoke-RestMethod -Uri "http://localhost:5262/api/transactions" `
-Method Post `
-ContentType "application/json" `
-Body '{"assetCode": "CRI_STEFANINI_005", "issuer": "Stefanini Capital", "price": 60000.00}'
```

---

### 📥 Consultar dados

```bash
Invoke-RestMethod -Uri "http://localhost:5262/api/transactions" -Method Get
```

---

## 📊 Diferenciais Técnicos

✔ Arquitetura Event-Driven aplicada na prática
✔ Integração entre múltiplas linguagens (.NET + Python)
✔ Comunicação assíncrona com fila (SQS)
✔ Ambiente cloud simulado localmente (LocalStack)
✔ Separação clara entre Producer e Consumer

---

## 🧩 Desafios Reais e Soluções

Este projeto aborda problemas comuns enfrentados em ambientes reais:

### 🔸 Serialização JSON inconsistente

Diferença entre CamelCase (.NET) e PascalCase (Python)
✔ Solução: fallback para múltiplos formatos

---

### 🔸 Limitação do EF Core

`EnsureCreated()` não aplica mudanças em tabelas existentes
✔ Solução: evolução manual de schema via SQL

---

### 🔸 Problemas com driver PostgreSQL

`psycopg2` apresentou inconsistências
✔ Solução: migração para `psycopg v3`

---

### 🔸 Execução SQL via PowerShell

Problemas com aspas e escaping
✔ Solução: execução direta dentro do container

---

## 📈 O que este projeto demonstra

Este projeto evidencia competências em:

* Arquitetura de sistemas distribuídos
* Engenharia de dados (ETL)
* Integração entre serviços
* Docker e ambientes isolados
* Debugging de problemas reais
* Boas práticas de backend

---

## 🔮 Possíveis Evoluções

* Deploy real na AWS (SQS, RDS, ECS ou Lambda)
* Implementação de Dead Letter Queue (DLQ)
* Observabilidade (logs, tracing)
* Retry strategy com backoff
* Autenticação e segurança

---

## 👨‍💻 Autor

**Vinicius Hoffmann**

* GitHub: https://github.com/viniciushoffmanndev
* LinkedIn: www.linkedin.com/in/viniciushoffmanndev

---

## ⭐ Destaque

Este projeto foi desenvolvido com foco em demonstrar **capacidade real de engenharia**, indo além de exemplos básicos e abordando problemas comuns de sistemas distribuídos.

👉 Ideal para avaliação técnica e portfólio profissional.
