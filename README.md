# Desafio - Produtos (React + C#)

Aplicacao web com:
- Login (autenticacao simples com JWT)
- Cadastro, listagem e exclusao de produtos (nome, preco e categoria)
- API REST em ASP.NET Core
- Frontend em React + TypeScript
- Pipeline basica de CI no GitHub Actions

## Stack

- Backend: ASP.NET Core 8 Web API
- Frontend: React + TypeScript + Vite
- Auth: JWT
- Execucao local: IIS (sem Docker)

## Estrutura

- `backend/Desafio.API`: API C#
- `frontend`: aplicacao React
- `scripts/sql`: scripts para criar banco e tabela no SQL Server
- `.github/workflows/ci.yml`: pipeline de build

### Frontend (Clean Architecture)

Pastas em `frontend/src`:

| Camada | Pasta | Papel |
|--------|--------|--------|
| **Domain** | `domain/entities/` | Tipos puros (produto, auth); sem React nem HTTP. |
| **Application** | `application/ports/` | Contratos (`AuthRepository`, `ProductRepository`, `TokenStorage`). |
| **Infrastructure** | `infrastructure/` | HTTP (Axios), repositorios que implementam os ports, `LocalTokenStorage`, `composition-root.ts` que monta as dependencias. |
| **Presentation** | `presentation/` | UI (paginas, estilos, roteamento), `AppDependenciesContext` injeta as implementacoes na arvore React. |

Regra de dependencia: **Presentation** e **Infrastructure** dependem de **Application** e **Domain**; **Domain** nao depende de nada. Telas usam apenas os *ports* via `useAppDependencies()`, nunca Axios direto.

**DTOs no front:** tipos em `infrastructure/http/dtos/` descrevem o JSON da API; `infrastructure/mappers/` converte DTO para entidade de dominio. Assim mudancas de contrato HTTP ficam isoladas da UI e do dominio.

## Banco de dados (SQL Server Express)

1. Abra o **SQL Server Management Studio** (ou `sqlcmd`) conectado na sua instancia (ex.: `.\SQLEXPRESS`).
2. Execute na ordem:
   - `scripts/sql/01_create_database.sql` — cria o banco `teste_db`
   - `scripts/sql/02_create_products_table.sql` — cria a tabela `dbo.Products` e um registro inicial opcional
   - `scripts/sql/03_create_users_table.sql` — cria a tabela `dbo.Users` (login; senha so como hash)

3. Ajuste a connection string em `backend/Desafio.API/appsettings.json` (ou `appsettings.Development.json`) se o nome da instancia for diferente:

```text
Server=.\\SQLEXPRESS;Database=teste_db;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Exemplos de `Server`:
- Instancia nomeada padrao Express: `.\\SQLEXPRESS` ou `localhost\\SQLEXPRESS`
- Instancia padrao na maquina: `.` ou `localhost`

## Login e usuarios

- A senha **nunca** fica no codigo: so existe **hash** na tabela `dbo.Users` (formato ASP.NET Core `PasswordHasher`, PBKDF2).
- **Primeiro acesso:** com a tabela `Users` vazia, a tela de login exibe o link **Criar usuario administrador**, que chama `POST /auth/register`. Depois que existe pelo menos um usuario, o cadastro publico e **fechado** (use apenas login).
- `GET /auth/registration-open` retorna `{ canRegister: true }` quando nao ha usuarios (o front usa isso).
- **SeedAdmin (opcional):** em `appsettings.Development.json`, `SeedAdmin.Enabled: true` cria um usuario ao subir a API. Nao use ao mesmo tempo que o cadastro na tela: ou o seed preenche o banco, ou voce cria pelo formulario. Por padrao o seed esta **desligado** em Development para favorecer o fluxo da tela.
- Em **producao**, mantenha `SeedAdmin.Enabled: false` em `appsettings.json`.

## Como rodar local (desenvolvimento)

### 1) Backend

```bash
cd backend/Desafio.API
dotnet restore
dotnet run
```

A API sobe por padrao em `http://localhost:5000` (ajuste se sua configuracao usar outra porta).

### 2) Frontend

```bash
cd frontend
copy .env.example .env
npm install
npm run dev
```

Frontend em `http://localhost:5173`.

## Endpoints

- `GET /auth/registration-open`
- `POST /auth/register` (so aceita quando nao existe nenhum usuario)
- `POST /auth/login`
- `GET /products` (auth)
- `POST /products` (auth)
- `DELETE /products/{id}` (auth)

## Publicacao no IIS (resumo)

## Backend (API)

1. Publicar:

```bash
cd backend/Desafio.API
dotnet publish -c Release -o ./publish
```

2. No IIS:
   - Criar site/aplicacao apontando para a pasta `publish`
   - Garantir que o Hosting Bundle do .NET 8 esteja instalado

## Frontend (React)

1. Build:

```bash
cd frontend
npm install
npm run build
```

2. Publicar no IIS:
   - Apontar site para pasta `frontend/dist`
   - Garantir que `web.config` foi copiado para `dist` (Vite copia arquivos de `public/`)

## CI

Workflow em `.github/workflows/ci.yml` com:
- Build do backend (.NET 8)
- Build do frontend (Node 20)

## Observacoes

- Produtos sao persistidos no SQL Server (EF Core), banco `teste_db`, tabela `dbo.Products`.
