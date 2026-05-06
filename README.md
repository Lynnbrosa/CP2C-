# Banco Digital — API

## 1. Identificação

| Nome | RM |
|------|----|
| Lynn Bueno Rosa | RM551102 |
| Gustavo Oliveira de Moura | RM555827 |
| Giovanne Charelli Zaniboni Silva | RM556223 |

## 2. Produtos bancários escolhidos e justificativas

**Empréstimo Pessoal** — produto mais representativo para demonstrar regra de negócio assíncrona: análise de crédito por score calculado a partir da idade e renda do cliente (PF). Reprova automaticamente clientes com score abaixo do mínimo configurado ou fora do range de valor/prazo.

**Máquina de Cartão** — produto com taxa MDR variável por ramo de atividade da empresa (PJ). Negócios de alimentação pagam +0,5%, varejo +0,3% e serviços +0,7% sobre a taxa base, demonstrando regra de negócio diferenciada para pessoa jurídica.

**Regra de negócio extra (trio):** Score de crédito para empréstimo calculado com fórmula `score = (idade - 18) × 5 + renda / 500`. Taxa MDR variável para máquina de cartão baseada no ramo de atividade da PJ.

## 4. Diagrama de Classes

> Coloque aqui a imagem após gerar no draw.io (arquivo: `/docs/diagrama-classes.drawio`)

```
Cliente (abstract, TPH discriminator=TipoCliente)
├── PessoaFisica  [CPF, DataNascimento, Renda]
└── PessoaJuridica [CNPJ, RazaoSocial, RamoAtividade]

Agencia ─── 1:N ─── Cliente ─── 1:N ─── Contratacao ─── N:1 ─── Produto (abstract)
                                                                    ├── Emprestimo
                                                                    ├── MaquinaDeCartao
                                                                    └── ReceberSalario
```

## 6. Endpoints

### POST /api/agencias — Cadastra agência
**Request:**
```json
{
  "nome": "Agência Centro",
  "codigo": "0001",
  "endereco": "Av. Paulista, 1000"
}
```
**Response 201:**
```json
{ "id": 1, "nome": "Agência Centro", "codigo": "0001", "endereco": "Av. Paulista, 1000" }
```

---

### POST /api/clientes/pf — Cadastra pessoa física
**Request:**
```json
{
  "nome": "João Silva",
  "email": "joao@email.com",
  "telefone": "11999998888",
  "agenciaId": 1,
  "cpf": "123.456.789-00",
  "dataNascimento": "1990-05-15",
  "renda": 5000.00
}
```
**Response 201:**
```json
{ "id": 1, "nome": "João Silva", "cpf": "12345678900", "renda": 5000.00, "agenciaId": 1 }
```
**Response 409 (CPF duplicado):**
```json
{ "erro": "CPF já cadastrado." }
```
**Response 404 (agência inexistente):**
```json
{ "erro": "Agência não encontrada." }
```

---

### POST /api/clientes/pj — Cadastra pessoa jurídica
**Request:**
```json
{
  "nome": "Empresa XYZ",
  "email": "contato@empresa.com",
  "agenciaId": 1,
  "cnpj": "12.345.678/0001-90",
  "razaoSocial": "Empresa XYZ Ltda",
  "ramoAtividade": "varejo"
}
```
**Response 201:**
```json
{ "id": 2, "nome": "Empresa XYZ", "cnpj": "12345678000190", "razaoSocial": "Empresa XYZ Ltda" }
```

---

### GET /api/clientes/{id} — Busca cliente por ID
**Response 200:**
```json
{ "id": 1, "nome": "João Silva", "email": "joao@email.com", "agencia": { "id": 1, "nome": "Agência Centro" } }
```

---

### GET /api/agencias/{id} — Busca agência por ID
**Response 200:**
```json
{ "id": 1, "nome": "Agência Centro", "codigo": "0001" }
```

---

### POST /api/contratacoes — Solicita contratação (publica na fila RabbitMQ)
**Request (Empréstimo):**
```json
{
  "clienteId": 1,
  "produtoId": 1,
  "valorSolicitado": 10000.00,
  "prazoMeses": 24
}
```
**Response 202:**
```json
{ "id": 1, "clienteId": 1, "produtoId": 1, "status": 0, "dataSolicitacao": "2026-05-05T21:00:00Z" }
```
**Response 404 (cliente inexistente):**
```json
{ "erro": "Cliente não encontrado." }
```

---

### GET /api/contratacoes/{id} — Consulta status da contratação
**Response 200 (aprovada):**
```json
{
  "id": 1,
  "status": 2,
  "observacao": "Aprovado. Score: 325. Taxa: 2.5% a.m.",
  "score": 325,
  "dataProcessamento": "2026-05-05T21:00:01Z"
}
```

**Status:** `0=Pendente | 1=EmAnalise | 2=Aprovada | 3=Reprovada`

## 7. Como executar os testes

```bash
dotnet test
```

```
Total tests: 9
     Passed: 9
 Total time: 1.08 Seconds
```

> _Coloque aqui o print do resultado do `dotnet test`._

## 8. Print do painel RabbitMQ

> _Coloque aqui o print do painel `http://localhost:15672` mostrando a fila `contratacoes`._

## 9. Print da API no Swagger com contratação aprovada

> _Coloque aqui o print do Swagger (`http://localhost:5000/swagger`) com a contratação aprovada._

---

## Como executar o projeto

### Pré-requisitos
- .NET 8.0 SDK
- Docker (para RabbitMQ)
- Credenciais Oracle FIAP

### 1. Suba o RabbitMQ
```bash
docker run -d --hostname rabbitmq --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```

### 2. Configure sua credencial Oracle
Edite `BancoDigital.API/appsettings.json`:
```json
"OracleConnection": "User Id=RM000000;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL"
```

### 3. Aplique as migrations
```bash
dotnet ef database update --project BancoDigital.API
```

### 4. Execute a API
```bash
dotnet run --project BancoDigital.API
```
Acesse: http://localhost:5000/swagger
