
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using CursaAqui_API.Data;
using CursaAqui_API.Infra;
using CursaAqui_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ---------- Banco ----------
builder.Services.AddDbContext<CursaAquiDbContext>(o =>
    o.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")
    ));

// ---------- MVC / JSON ----------
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        ));

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Informe o token JWT."
    });

    c.AddSecurityRequirement(document =>
        new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] =
                new List<string>()
        });
});

// ---------- Serviços ----------
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<MatriculaService>();

// ---------- JWT ----------
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Key com pelo menos 32 caracteres " +
        "(use 'dotnet user-secrets' ou variável de ambiente Jwt__Key)."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            RoleClaimType = "role",
            NameClaimType = "name",

            ClockSkew = TimeSpan.FromMinutes(1)
        };

        // Token de usuário inativo ou excluído deixa de funcionar
        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var db = ctx.HttpContext
                    .RequestServices
                    .GetRequiredService<CursaAquiDbContext>();

                var sub = ctx.Principal?
                    .FindFirst("sub")?
                    .Value;

                if (!int.TryParse(sub, out var id))
                {
                    ctx.Fail("Usuário inválido.");
                    return;
                }

                var usuarioAtivo = await db.Usuarios
                    .AnyAsync(u => u.Id == id && u.Ativo);

                if (!usuarioAtivo)
                {
                    ctx.Fail("Usuário inativo ou inexistente.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS ----------
var origens =
    builder.Configuration
        .GetSection("Cors:Origens")
        .Get<string[]>()
    ?? Array.Empty<string>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("Padrao", p =>
    {
        p.WithOrigins(origens)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ---------- Rate Limit ----------
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    o.AddPolicy(
        "login",
        ctx =>
            RateLimitPartition.GetFixedWindowLimiter(
                ctx.Connection.RemoteIpAddress?.ToString()
                    ?? "anonimo",

                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1)
                }
            )
    );
});

var app = builder.Build();

// ---------- Swagger ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ---------- Migrations + Seed ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<CursaAquiDbContext>();

    await db.Database.MigrateAsync();

    await DbSeeder.ExecutarAsync(
        db,
        app.Configuration
    );
}

// ---------- Tratamento de erros + HTTPS ----------
// HTTPS somente em produção.
// Em desenvolvimento, a API roda em:
// http://localhost:5035

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(a =>
        a.Run(async ctx =>
        {
            ctx.Response.StatusCode = 500;

            await ctx.Response.WriteAsJsonAsync(
                new
                {
                    erro = "Erro interno. Tente novamente."
                }
            );
        })
    );

    app.UseHsts();
    app.UseHttpsRedirection();
}

// ---------- Arquivos ----------
var pastaUploads = Arquivos.Pasta(
    app.Configuration,
    app.Environment
);

Directory.CreateDirectory(pastaUploads);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            pastaUploads
        ),

        RequestPath = "/arquivos"
    }
);

// ---------- CORS ----------
app.UseCors("Padrao");

// ---------- Rate Limiter ----------
app.UseRateLimiter();

// ---------- Autenticação ----------
app.UseAuthentication();

// ---------- Autorização ----------
app.UseAuthorization();

// ---------- Controllers ----------
app.MapControllers();

// ---------- Iniciar API ----------
app.Run();
