# Publicando a Cursa-API no Azure (gratuito)

Usa a assinatura **Azure for Students** com recursos que não consomem crédito:
**Azure SQL Database (oferta gratuita)** + **App Service plano F1 (Free)**.

O nome usado no código é `cursaaqui-api` (URL `https://cursaaqui-api.azurewebsites.net`).
Se esse nome já estiver em uso, escolha outro e troque-o em:
- `.github/workflows/deploy-api.yml` (`AZURE_WEBAPP_NAME`)
- `site_cursaaqui/utils.js` (`API_BASE_URL`)
- `CursaAqui.App/api.config.json` (`BaseUrl`)

## 1. Banco — Azure SQL Database

1. Portal → **SQL databases** → *Create*.
2. Clique em **Apply offer** (oferta gratuita) no topo.
3. Crie um servidor novo com **SQL authentication** (anote usuário e senha).
   Use uma região permitida para estudantes (ex.: *East US 2*, se *Brazil South* não aparecer).
4. Em *Behavior when free limit reached* escolha
   **Auto-pause the database until next month** (assim nunca gera cobrança).
5. Após criar, no **servidor** → *Networking*:
   - marque **Allow Azure services and resources to access this server**;
   - (opcional) adicione o seu IP para acessar pelo SSMS/Azure Data Studio.
6. No banco → *Connection strings* → copie a **ADO.NET** e coloque a senha.

As tabelas são criadas automaticamente: a API roda `Database.MigrateAsync()` ao iniciar.

## 2. API — App Service

1. Portal → **App Services** → *Create* → *Web App*.
2. Nome: `cursaaqui-api` · Publish: **Code** · Runtime: **.NET 8 (LTS)** · OS: **Linux**.
3. Pricing plan: crie um plano **Free F1**.
4. Depois de criado → *Settings* → **Environment variables** → adicione:

   | Nome | Valor |
   |---|---|
   | `ConnectionStrings__Default` | connection string do passo 1 |
   | `Jwt__Key` | texto aleatório com **32+ caracteres** |
   | `Uploads__Pasta` | `/home/data/uploads` |

   `/home` é o armazenamento persistente do App Service; assim os uploads não
   são apagados a cada deploy.

5. *Settings* → *Configuration* → **SCM Basic Auth Publishing Credentials: On**
   (necessário para baixar o publish profile).

## 3. Deploy automático pelo GitHub

1. No App Service → *Overview* → **Download publish profile**.
2. No GitHub → *Settings* → *Secrets and variables* → *Actions* →
   novo secret `AZURE_WEBAPP_PUBLISH_PROFILE` com o conteúdo do arquivo.
3. Faça push na `main` (ou rode o workflow **Deploy API to Azure App Service** manualmente).

Teste: `https://cursaaqui-api.azurewebsites.net/api/...`
(Swagger só aparece em ambiente Development.)

## Observações do plano gratuito

- A API "dorme" quando fica sem uso e o banco pausa: a primeira requisição pode levar
  de alguns segundos a ~1 min. A API já tenta reconectar ao banco automaticamente.
- F1 permite 60 min de CPU por dia — suficiente para apresentação/testes.
- Para rodar o app desktop contra a API local, defina a variável de ambiente
  `CURSAAQUI_API_URL=http://localhost:5035` (ela tem prioridade sobre o `api.config.json`).
