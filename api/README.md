# Cursa Aqui — API

API REST em Express.js + Prisma (SQL Server) que substitui o `localStorage` usado anteriormente pelo frontend estático em `site_cursaaqui/`.

## Pré-requisitos

- Node.js 18+ (instale em https://nodejs.org)
- SQL Server (Express ou LocalDB — veja abaixo)

## 1. Banco de dados

**Nota importante descoberta na prática:** o driver do Prisma para SQL Server (`sqlserver://host:port;...`) só conecta via **TCP**, e não entende a sintaxe especial `(localdb)\instancia` nem caminhos de named pipe. O **LocalDB** por padrão só escuta em named pipe/memória compartilhada, e habilitar TCP nele acaba exigindo os mesmos privilégios de administrador que uma instância completa — então, na prática, **é mais simples usar uma instância normal do SQL Server Express** (se já tiver uma instalada, como costuma vir em instalações do Visual Studio/SSMS) do que brigar com o LocalDB.

Se já existe uma instância `SQLEXPRESS` parada na máquina (`Get-Service 'MSSQL$SQLEXPRESS'`), rode o script `setup-sqlserver.ps1` na raiz do repositório **como Administrador** (botão direito → "Executar com o PowerShell", aceitar o UAC). Ele faz tudo de uma vez:
1. Habilita o protocolo TCP/IP na instância, porta fixa `1433`.
2. Coloca o serviço como automático e o reinicia.
3. Libera a porta `1433` no Firewall do Windows (regra local).
4. Cria o login `cursaaqui_dev` (senha `CursaAqui123!`) e o banco `cursaaqui`.

Isso só precisa de aprovação manual porque iniciar serviços do Windows e mexer na configuração de rede do SQL Server exige elevação — não tem como automatizar sem essa etapa.

Se preferir configurar na mão (ou não tiver uma instância SQLEXPRESS pronta), siga os mesmos passos via *SQL Server Configuration Manager* (habilitar TCP/IP, porta 1433) e crie o login/banco com `sqlcmd`:

```sql
CREATE LOGIN cursaaqui_dev WITH PASSWORD = 'CursaAqui123!', CHECK_POLICY = OFF;
CREATE DATABASE cursaaqui;
GO
USE cursaaqui;
CREATE USER cursaaqui_dev FOR LOGIN cursaaqui_dev;
ALTER ROLE db_owner ADD MEMBER cursaaqui_dev;
```

(Login SQL dedicado em vez de autenticação integrada do Windows — suporte do Prisma a Integrated Security no connector `sqlserver` é inconsistente entre versões.)

## 2. Configurar o `.env`

```bash
cp .env.example .env
```

Se você rodou o `setup-sqlserver.ps1`, o `.env` já commitado localmente (não versionado) já aponta para `localhost:1433` com o usuário/senha criados pelo script — só conferir que bate. Caso configure na mão, ajuste `DATABASE_URL` com a porta/senha que você escolheu, e defina um `JWT_SECRET` próprio.

## 3. Instalar dependências, migrar e popular o banco

```bash
npm install
npx prisma migrate dev --name init
npx prisma db seed
```

O seed recria o catálogo de ~12 cursos (a partir de `site_cursaaqui/courses.js`) e as duas contas demo:

| E-mail | Senha | Papel |
|---|---|---|
| ricardo@cursaaqui.com | 123456 | Professor |
| aluno@cursaaqui.com | 123456 | Aluno |

## 4. Rodar a API

```bash
npm run dev
```

API disponível em `http://localhost:3001` (ajustável via `PORT` no `.env`). `GET /health` confirma que está no ar.

## 5. Servir o frontend

Como o frontend agora faz `fetch()` para a API, abrir `site_cursaaqui/index.html` direto como `file://` não funciona (CORS bloqueia). Sirva via um servidor estático local, por exemplo:

```bash
npx serve site_cursaaqui
# ou: python -m http.server 5500 (dentro de site_cursaaqui/)
```

Garanta que `CORS_ORIGIN` no `.env` da API corresponda à origem usada (ex: `http://localhost:5500`).

## Papéis de usuário

O campo `role` de `User` aceita `STUDENT`, `TEACHER`, `ADMIN` e `SECRETARIA` (validado na aplicação, não como enum do banco — o connector `sqlserver` do Prisma não suporta enums nativos). Nesta etapa só `STUDENT` e `TEACHER` têm rotas implementadas — os outros dois ficam reservados para uso futuro sem exigir nova migração de banco.

## Prisma Studio (inspecionar o banco)

```bash
npx prisma studio
```
