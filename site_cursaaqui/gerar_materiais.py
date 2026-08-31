# -*- coding: utf-8 -*-
"""
gerar_materiais.py - Gerador Oficial e Autônomo de Apostilas do Cursa Aqui
Cria 15 PDFs aprofundados, formatados com paginação profissional, tabelas,
códigos-fonte, estudos de caso e questões comentadas para cada curso da plataforma.
"""

import os
import re
from reportlab.lib.pagesizes import letter
from reportlab.lib import colors
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, HRFlowable, Table, TableStyle, PageBreak, KeepTogether
)
from reportlab.pdfgen import canvas

os.makedirs("materiais", exist_ok=True)

class PaginatedCanvas(canvas.Canvas):
    def __init__(self, *args, **kwargs):
        super(PaginatedCanvas, self).__init__(*args, **kwargs)
        self._saved_page_states = []

    def showPage(self):
        self._saved_page_states.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        num_pages = len(self._saved_page_states)
        for state in self._saved_page_states:
            self.__dict__.update(state)
            self.draw_page_decorations(num_pages)
            super(PaginatedCanvas, self).showPage()
        super(PaginatedCanvas, self).save()

    def draw_page_decorations(self, page_count):
        self.saveState()
        
        # Cabeçalho a partir da página 2
        if self._pageNumber > 1:
            self.setFont("Helvetica-Bold", 8)
            self.setFillColor(colors.HexColor("#2563EB"))
            self.drawString(40, 762, "🎓 CURSA AQUI")
            self.setFont("Helvetica", 8)
            self.setFillColor(colors.HexColor("#64748B"))
            self.drawString(110, 762, "— Material Didático Oficial de Ensino e Especialização Profissional")
            self.setStrokeColor(colors.HexColor("#CBD5E1"))
            self.setLineWidth(0.75)
            self.line(40, 754, 572, 754)
        
        # Rodapé em todas as páginas
        self.setStrokeColor(colors.HexColor("#CBD5E1"))
        self.setLineWidth(0.75)
        self.line(40, 42, 572, 42)
        
        self.setFont("Helvetica", 8)
        self.setFillColor(colors.HexColor("#64748B"))
        self.drawString(40, 30, "© 2026 Cursa Aqui • Apostila Oficial para Alunos Matriculados • Proibida a reprodução")
        
        self.setFont("Helvetica-Bold", 8)
        self.setFillColor(colors.HexColor("#1E293B"))
        page_text = f"Página {self._pageNumber} de {page_count}"
        self.drawRightString(572, 30, page_text)
        
        self.restoreState()


base_styles = getSampleStyleSheet()
primary = colors.HexColor("#2563EB")
dark = colors.HexColor("#0F172A")
body_text = colors.HexColor("#334155")

styles = {
    "CoverTitle": ParagraphStyle('CoverTitle', parent=base_styles['Heading1'], fontSize=20, leading=24, textColor=primary, fontName="Helvetica-Bold", spaceAfter=4),
    "CoverSubtitle": ParagraphStyle('CoverSubtitle', parent=base_styles['Normal'], fontSize=11, leading=15, textColor=dark, fontName="Helvetica-Bold", spaceAfter=8),
    "CoverMeta": ParagraphStyle('CoverMeta', parent=base_styles['Normal'], fontSize=8.5, leading=12, textColor=colors.HexColor("#475569"), fontName="Helvetica"),
    "ModuleH1": ParagraphStyle('ModuleH1', parent=base_styles['Heading1'], fontSize=13, leading=17, textColor=primary, fontName="Helvetica-Bold", spaceBefore=10, spaceAfter=4),
    "SubH2": ParagraphStyle('SubH2', parent=base_styles['Heading2'], fontSize=10, leading=14, textColor=dark, fontName="Helvetica-Bold", spaceBefore=6, spaceAfter=2),
    "Body": ParagraphStyle('Body', parent=base_styles['Normal'], fontSize=8.6, leading=13, textColor=body_text, fontName="Helvetica", spaceAfter=4.5),
    "Bullet": ParagraphStyle('Bullet', parent=base_styles['Normal'], fontSize=8.4, leading=12.5, textColor=body_text, fontName="Helvetica", leftIndent=12, spaceAfter=2.5),
    "Code": ParagraphStyle('Code', parent=base_styles['Code'], fontSize=7.6, leading=10.2, textColor=colors.HexColor("#0F172A"), fontName="Courier", spaceBefore=2, spaceAfter=2),
    "BoxText": ParagraphStyle('BoxText', parent=base_styles['Normal'], fontSize=8.2, leading=12, textColor=dark, fontName="Helvetica"),
    "BoxTitle": ParagraphStyle('BoxTitle', parent=base_styles['Normal'], fontSize=8.8, leading=12.5, textColor=primary, fontName="Helvetica-Bold", spaceAfter=2),
    "TableHead": ParagraphStyle('TH', parent=base_styles['Normal'], fontSize=8, leading=10, textColor=colors.white, fontName="Helvetica-Bold"),
    "TableCell": ParagraphStyle('TC', parent=base_styles['Normal'], fontSize=7.6, leading=10.2, textColor=body_text, fontName="Helvetica")
}

def sanitize(text):
    if not isinstance(text, str):
        return str(text)
    allowed = ['b', '/b', 'i', '/i', 'u', '/u', 'br', 'br/', 'font', '/font', 'sup', '/sup', 'sub', '/sub']
    def replacer(match):
        tag_content = match.group(1).strip()
        tag_name = tag_content.split()[0].lower()
        if tag_name in allowed:
            return match.group(0)
        return f"&lt;{tag_content}&gt;"
    return re.sub(r'<([^>]+)>', replacer, text)

def P(text, style_name="Body"):
    return Paragraph(sanitize(text), styles[style_name])

def make_box(title, text, bg="#EFF6FF", border="#93C5FD"):
    data = [[P(title, "BoxTitle")], [P(text, "BoxText")]]
    t = Table(data, colWidths=[532])
    t.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,-1), colors.HexColor(bg)),
        ('BOX', (0,0), (-1,-1), 0.8, colors.HexColor(border)),
        ('PADDING', (0,0), (-1,-1), 6),
        ('TOPPADDING', (0,0), (-1,-1), 4),
        ('BOTTOMPADDING', (0,0), (-1,-1), 4),
    ]))
    return t

def make_code(code_text):
    escaped = (
        code_text.replace("&", "&amp;")
                 .replace("<", "&lt;")
                 .replace(">", "&gt;")
                 .replace(" ", "&nbsp;")
                 .replace("\n", "<br/>")
    )
    data = [[Paragraph(escaped, styles["Code"])]]
    t = Table(data, colWidths=[532])
    t.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,-1), colors.HexColor("#F1F5F9")),
        ('BOX', (0,0), (-1,-1), 0.75, colors.HexColor("#CBD5E1")),
        ('PADDING', (0,0), (-1,-1), 6),
        ('LEFTPADDING', (0,0), (-1,-1), 10),
    ]))
    return t

def make_table(headers, rows, col_widths=None):
    h_row = [P(h, "TableHead") for h in headers]
    b_rows = [[P(cell, "TableCell") for cell in r] for r in rows]
    data = [h_row] + b_rows
    if not col_widths:
        col_widths = [532 / len(headers)] * len(headers)
    t = Table(data, colWidths=col_widths)
    t.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), primary),
        ('ROWBACKGROUNDS', (0,1), (-1,-1), [colors.HexColor("#FFFFFF"), colors.HexColor("#F8FAFC")]),
        ('GRID', (0,0), (-1,-1), 0.5, colors.HexColor("#CBD5E1")),
        ('PADDING', (0,0), (-1,-1), 4.5),
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE'),
    ]))
    return t

# ============================================================
# CATÁLOGO DE CONTEÚDO DOS 15 CURSOS
# ============================================================
courses_data = [
    {
        "id": 1,
        "filename": "materiais/ads_manual_academico.pdf",
        "title": "Análise e Desenvolvimento de Sistemas (ADS)",
        "subtitle": "Manual Acadêmico, Engenharia de Requisitos, POO e Qualidade de Software",
        "duration": "40 horas",
        "level": "Superior de Tecnologia",
        "category": "Engenharia de Software",
        "author": "Coordenação Acadêmica — Cursa Aqui",
        "modules": [
            {
                "title": "1. Engenharia de Requisitos e Modelagem Conceitual",
                "blocks": [
                    ("text", ["A Engenharia de Requisitos é a espinha dorsal de qualquer projeto de software bem-sucedido. Requisitos mal definidos respondem por mais de 70% das falhas de entrega em projetos de TI.",
                             "<b>Classificação dos Requisitos de Software:</b>",
                             "• <b>Requisitos Funcionais (RF):</b> Descrevem as funções e serviços diretos que o sistema deve fornecer (ex: cadastrar alunos, processar pagamentos via PIX, emitir certificados).",
                             "• <b>Requisitos Não-Funcionais (RNF):</b> Expressam restrições de qualidade sobre os serviços oferecidos (ex: tempo de resposta < 500ms, criptografia TLS 1.3, disponibilidade 99.9%).",
                             "• <b>Regras de Negócio (RN):</b> Políticas corporativas ou leis (ex: LGPD) que governam o negócio."]),
                    ("callout", ("⚠️ Restrição de Projeto vs Meta de Usuário", "O desejo do cliente por respostas rápidas é uma Meta de Usuário. Já o limite imposto por um banco legado (ex: tempo mínimo de 3s) é uma Restrição de Projeto que delimita a arquitetura final.")),
                    ("table", (["Tipo de Requisito", "Foco Principal", "Exemplo Prático"], [
                        ["Funcional (RF)", "O que o software executa", "Pesquisar cursos e calcular média final do aluno."],
                        ["Não-Funcional (RNF)", "Critérios de qualidade/desempenho", "Suportar 10.000 requisições concorrentes sem degradação."],
                        ["Restrição de Projeto", "Limitações técnicas duras", "Obrigatório hospedar em nuvem brasileira em conformidade com LGPD."]
                    ]))
                ]
            },
            {
                "title": "2. Paradigma de Orientação a Objetos (POO) & SOLID",
                "blocks": [
                    ("text", ["A Orientação a Objetos organiza o software em entidades conceituais (objetos) com estado e comportamento.",
                             "<b>Os Quatro Pilares:</b>",
                             "1. <b>Abstração:</b> Isolar as características essenciais do domínio.",
                             "2. <b>Encapsulamento:</b> Ocultar detalhes internos e proteger o estado com modificadores de acesso.",
                             "3. <b>Herança:</b> Especialização e reaproveitamento de código entre classes.",
                             "4. <b>Polimorfismo:</b> Capacidade de subclasses responderem à mesma mensagem de maneiras distintas."]),
                    ("code", "public interface IRepositorioCurso {\n    Task<Curso> ObterPorIdAsync(int id);\n}\n\npublic class RepositorioCursoSql : IRepositorioCurso {\n    public async Task<Curso> ObterPorIdAsync(int id) {\n        // Consulta otimizada com EF Core / Dapper\n    }\n}")
                ]
            },
            {
                "title": "3. Pirâmide de Testes e Garantia da Qualidade (QA)",
                "blocks": [
                    ("text", ["• <b>Verificação:</b> <i>'Estamos construindo o produto corretamente (seguindo as normas)?'</i>",
                             "• <b>Validação:</b> <i>'Estamos construindo o produto certo (atendendo o cliente)?'</i>",
                             "• <b>Pirâmide de Testes:</b> 70% Testes Unitários (rápidos e baratos), 20% Testes de Integração e 10% Testes E2E (interface)."])
                ]
            }
        ]
    },
    {
        "id": 2,
        "filename": "materiais/cloud_computing_devops.pdf",
        "title": "Cloud Computing e DevOps",
        "subtitle": "Infraestrutura em Nuvem, Docker, Kubernetes, CI/CD e Terraform",
        "duration": "60 horas",
        "level": "Intermediário / Avançado",
        "category": "Infraestrutura & Nuvem",
        "author": "Prof. Ricardo Silva — Cursa Aqui",
        "modules": [
            {
                "title": "1. Modelos de Serviço em Nuvem (IaaS, PaaS, SaaS, FaaS)",
                "blocks": [
                    ("text", ["A Nuvem substitui gastos de capital em data centers (CapEx) por gastos operacionais sob demanda (OpEx).",
                             "• <b>IaaS:</b> Servidores virtuais e redes (AWS EC2, Azure VMs).",
                             "• <b>PaaS:</b> Plataformas gerenciadas onde você entrega apenas o código (AWS Elastic Beanstalk, Heroku).",
                             "• <b>SaaS:</b> Software completo pronto para uso (Google Workspace, M365).",
                             "• <b>FaaS / Serverless:</b> Execução de funções ativadas por eventos com cobrança por milissegundo (AWS Lambda)."])
                ]
            },
            {
                "title": "2. Containers Docker vs Máquinas Virtuais",
                "blocks": [
                    ("text", ["Diferente das VMs que virtualizam todo o hardware e o SO convidado, os Containers compartilham o kernel do SO hospedeiro, pesando poucos megabytes e iniciando em milissegundos."]),
                    ("code", "FROM node:20-alpine AS builder\nWORKDIR /app\nCOPY package*.json ./\nRUN npm ci\nCOPY . .\nRUN npm run build\n\nFROM node:20-alpine AS runner\nWORKDIR /app\nCOPY --from=builder /app/dist ./dist\nEXPOSE 3000\nCMD [\"node\", \"dist/main.js\"]")
                ]
            },
            {
                "title": "3. Orquestração com Kubernetes e CI/CD",
                "blocks": [
                    ("text", ["O Kubernetes automatiza deploy, escalabilidade horizontal (HPA) e auto-recuperação (Self-Healing) de clusters de containers.",
                             "Pipelines CI/CD (GitHub Actions / GitLab CI) executam testes automáticos e entregam código em produção com zero downtime."])
                ]
            }
        ]
    },
    {
        "id": 3,
        "filename": "materiais/desenv_mobile_js.pdf",
        "title": "Desenvolvimento Mobile com JavaScript",
        "subtitle": "Aplicações Nativas para iOS e Android com React Native e Expo",
        "duration": "50 horas",
        "level": "Intermediário",
        "category": "Desenvolvimento Mobile",
        "author": "Profª. Amanda Costa — Cursa Aqui",
        "modules": [
            {
                "title": "1. React Native e Componentes Nativos",
                "blocks": [
                    ("text", ["O React Native compila elementos declarativos em componentes nativos reais do iOS (UIKit) e Android (Views).",
                             "• <b>View:</b> Container fundamental equivalente à div.",
                             "• <b>Text:</b> Obrigatório para qualquer conteúdo textual.",
                             "• <b>FlatList:</b> Componente com reciclagem de memória para listas de alto rendimento."])
                ]
            },
            {
                "title": "2. Flexbox Mobile e Otimização de Listas",
                "blocks": [
                    ("text", ["No mobile, `flexDirection: column` é o padrão. Para listas grandes, o uso de `FlatList` é mandatório para evitar travamentos de rolagem (jank)."]),
                    ("code", "import { FlatList, Text, View } from 'react-native';\n\nexport function FeedCursos({ cursos }) {\n  return (\n    <FlatList\n      data={cursos}\n      keyExtractor={(item) => String(item.id)}\n      renderItem={({ item }) => <Text>{item.title}</Text>}\n      removeClippedSubviews={true}\n    />\n  );\n}")
                ]
            }
        ]
    },
    {
        "id": 4,
        "filename": "materiais/desenv_web_dotnet.pdf",
        "title": "Desenvolvimento Web com .NET & C#",
        "subtitle": "APIs Corporativas com ASP.NET Core, EF Core, JWT e Clean Architecture",
        "duration": "55 horas",
        "level": "Intermediário / Avançado",
        "category": "Desenvolvimento Web",
        "author": "Prof. Carlos Eduardo — Cursa Aqui",
        "modules": [
            {
                "title": "1. C# Moderno e Minimal APIs no .NET 8",
                "blocks": [
                    ("text", ["O .NET 8 entrega alto rendimento com Minimal APIs, injeção de dependência nativa e processamento assíncrono com `async/await`."]),
                    ("code", "var builder = WebApplication.CreateBuilder(args);\nbuilder.Services.AddScoped<ICursoRepository, CursoRepository>();\nvar app = builder.Build();\napp.MapGet(\"/api/cursos\", async (ICursoRepository repo) => Results.Ok(await repo.ListarAsync()));\napp.Run();")
                ]
            },
            {
                "title": "2. Entity Framework Core e Migrations",
                "blocks": [
                    ("text", ["As Migrations (`dotnet ef migrations add`) convertem alterações nas classes de modelo C# em instruções DDL versionáveis no banco de dados."])
                ]
            }
        ]
    },
    {
        "id": 5,
        "filename": "materiais/empreendedorismo_ti.pdf",
        "title": "Empreendedorismo em TI",
        "subtitle": "Criação de Startups, Business Model Canvas, MVP e Métricas SaaS",
        "duration": "30 horas",
        "level": "Fundamental / Intermediário",
        "category": "Gestão & Negócios",
        "author": "Profª. Patricia Lima — Cursa Aqui",
        "modules": [
            {
                "title": "1. O Business Model Canvas e Validação de MVP",
                "blocks": [
                    ("text", ["O Canvas sintetiza o modelo de negócio em 9 blocos estratégicos.",
                             "O <b>MVP (Produto Mínimo Viável)</b> permite validar hipóteses com o menor esforço e custo de desenvolvimento possível (Lean Startup)."])
                ]
            },
            {
                "title": "2. Métricas de Negócios SaaS",
                "blocks": [
                    ("text", ["• <b>CAC:</b> Custo de Aquisição de Clientes.",
                             "• <b>LTV:</b> Valor gerado pelo cliente ao longo da vida do contrato.",
                             "• <b>Regra de Ouro:</b> `LTV / CAC >= 3`.",
                             "• <b>Churn Rate:</b> Taxa percentual de cancelamento mensal."])
                ]
            }
        ]
    },
    {
        "id": 6,
        "filename": "materiais/relacoes_etnico_raciais.pdf",
        "title": "Relações Étnico-Raciais e Afrodescendência",
        "subtitle": "História Afro-Brasileira, Direitos Humanos, Lei 10.639 e Inclusão Corporativa",
        "duration": "40 horas",
        "level": "Superior / Extensão",
        "category": "Ciências Humanas & Sociais",
        "author": "Prof. Dr. Marcos Oliveira — Cursa Aqui",
        "modules": [
            {
                "title": "1. Marcos Históricos e Legislação (Lei nº 10.639/2003)",
                "blocks": [
                    ("text", ["A Lei nº 10.639/2003 tornou obrigatório o ensino da história e cultura afro-brasileira nas escolas, resgatando legados e combatendo o eurocentrismo."])
                ]
            },
            {
                "title": "2. Sociologia do Racismo e Vieses em Inteligência Artificial",
                "blocks": [
                    ("text", ["Classificação do racismo: Interpessoal, Institucional e Estrutural.",
                             "Atenção aos <b>Vieses Algorítmicos (Algorithmic Bias)</b> em modelos de reconhecimento facial e recrutamento por IA, exigindo equipes multidisciplinares e diversas."])
                ]
            }
        ]
    },
    {
        "id": 7,
        "filename": "materiais/redes_computadores.pdf",
        "title": "Redes de Computadores",
        "subtitle": "Modelo OSI, Pilha TCP/IP, Roteamento, Protocolos e Segurança",
        "duration": "45 horas",
        "level": "Fundamental / Intermediário",
        "category": "Redes & Segurança",
        "author": "Prof. Fernando Santos — Cursa Aqui",
        "modules": [
            {
                "title": "1. Modelo OSI de 7 Camadas vs TCP/IP",
                "blocks": [
                    ("text", ["1. Física ➔ 2. Enlace (Switches L2) ➔ 3. Rede (Roteadores / IP) ➔ 4. Transporte (TCP/UDP) ➔ 5. Sessão ➔ 6. Apresentação ➔ 7. Aplicação (HTTP/DNS).",
                             "Os roteadores atuam na <b>Camada 3 (Rede)</b> realizando encaminhamento por endereçamento IP."])
                ]
            },
            {
                "title": "2. TCP vs UDP",
                "blocks": [
                    ("text", ["• <b>TCP:</b> Confiável, ordenado com Three-Way Handshake (SYN ➔ SYN-ACK ➔ ACK).",
                             "• <b>UDP:</b> Rápido, sem conexão prévia, ideal para streaming e jogos."])
                ]
            }
        ]
    },
    {
        "id": 8,
        "filename": "materiais/ia_machine_learning_python.pdf",
        "title": "Inteligência Artificial e Machine Learning com Python",
        "subtitle": "Data Science com Pandas/NumPy, Modelos Supervisionados e Deep Learning",
        "duration": "65 horas",
        "level": "Intermediário / Avançado",
        "category": "Data Science & IA",
        "author": "Profª. Dra. Helena Zhang — Cursa Aqui",
        "modules": [
            {
                "title": "1. Manipulação de Dados com Pandas e NumPy",
                "blocks": [
                    ("text", ["Pandas é a ferramenta dominante para manipulação e limpeza de dados tabulares estruturados em DataFrames."]),
                    ("code", "import pandas as pd\nfrom sklearn.ensemble import RandomForestClassifier\n\ndf = pd.read_csv('dados_alunos.csv')\nX = df[['horas_estudo', 'exercicios']]\ny = df['aprovado']\n\nmodelo = RandomForestClassifier()\nmodelo.fit(X, y)")
                ]
            },
            {
                "title": "2. Métricas de Avaliação e Deep Learning",
                "blocks": [
                    ("text", ["Avaliação por Precisão, Recall e F1-Score.",
                             "Redes Neurais Convolucionais (CNNs) para Visão Computacional e Transformers para Modelos de Linguagem (LLMs)."])
                ]
            }
        ]
    },
    {
        "id": 9,
        "filename": "materiais/cybersecurity_pentest.pdf",
        "title": "Cybersecurity e Pentest Prático",
        "subtitle": "Segurança da Informação, OWASP Top 10, SQL Injection e Defesa Cibernética",
        "duration": "50 horas",
        "level": "Intermediário / Avançado",
        "category": "Segurança da Informação",
        "author": "Prof. Gabriel Vianna — Cursa Aqui",
        "modules": [
            {
                "title": "1. Fases de um Pentest Ético",
                "blocks": [
                    ("text", ["Reconhecimento (OSINT) ➔ Varredura (Nmap) ➔ Análise de Vulnerabilidades ➔ Exploração ➔ Pós-Exploração ➔ Relatório Executivo."])
                ]
            },
            {
                "title": "2. Vulnerabilidades OWASP Top 10 (SQL Injection e XSS)",
                "blocks": [
                    ("text", ["<b>SQL Injection (SQLi):</b> Execução de comandos no banco por falta de sanitização.",
                             "<b>Defesa:</b> Uso obrigatório de Consultas Parametrizadas (Prepared Statements)."]),
                    ("code", "// Consulta Segura com Prepared Statement\nconst sql = 'SELECT * FROM usuarios WHERE email = $1 AND ativo = $2';\nconst res = await pool.query(sql, [emailInput, true]);")
                ]
            }
        ]
    },
    {
        "id": 10,
        "filename": "materiais/ui_ux_design_figma.pdf",
        "title": "UI/UX Design e Prototipagem no Figma",
        "subtitle": "Pesquisa com Usuários, Heurísticas de Usabilidade e Design Systems",
        "duration": "35 horas",
        "level": "Fundamental / Intermediário",
        "category": "Design & Experiência do Usuário",
        "author": "Profª. Beatriz Mendes — Cursa Aqui",
        "modules": [
            {
                "title": "1. Fundamentos de UX e o Papel do Wireframe",
                "blocks": [
                    ("text", ["Um <b>Wireframe</b> é o esboço visual de baixa fidelidade que estrutura a hierarquia e o layout antes da aplicação das cores e do código."])
                ]
            },
            {
                "title": "2. Heurísticas de Usabilidade de Nielsen",
                "blocks": [
                    ("text", ["10 princípios fundamentais: Visibilidade do status, consistência, prevenção de erros, reconhecimento em vez de memorização e documentação clara."])
                ]
            }
        ]
    },
    {
        "id": 11,
        "filename": "materiais/banco_de_dados_sql_nosql.pdf",
        "title": "Banco de Dados SQL & NoSQL",
        "subtitle": "Modelagem Relacional (PostgreSQL), Consultas Avançadas e MongoDB",
        "duration": "45 horas",
        "level": "Intermediário",
        "category": "Banco de Dados",
        "author": "Prof. Lucas Albuquerque — Cursa Aqui",
        "modules": [
            {
                "title": "1. Modelagem Relacional e Formas Normais (1FN, 2FN, 3FN)",
                "blocks": [
                    ("text", ["Normalização para evitar redundâncias. Consultas com <b>JOIN</b> para combinar tabelas com base em chaves estrangeiras."]),
                    ("code", "SELECT c.titulo, COUNT(m.id) AS total_matriculas\nFROM cursos c\nINNER JOIN matriculas m ON c.id = m.curso_id\nGROUP BY c.titulo\nHAVING COUNT(m.id) > 10\nORDER BY total_matriculas DESC;")
                ]
            },
            {
                "title": "2. Transações ACID e NoSQL com MongoDB",
                "blocks": [
                    ("text", ["• <b>ACID:</b> Atomicidade, Consistência, Isolamento e Durabilidade.",
                             "• <b>MongoDB:</b> Armazenamento orientado a documentos flexíveis em formato BSON/JSON."])
                ]
            }
        ]
    },
    {
        "id": 12,
        "filename": "materiais/microservicos_nodejs_docker.pdf",
        "title": "Arquitetura de Microserviços com Node.js e Docker",
        "subtitle": "Sistemas Distribuídos, API Gateway, RabbitMQ e Observabilidade",
        "duration": "55 horas",
        "level": "Avançado",
        "category": "Backend",
        "author": "Prof. Thiago Martins — Cursa Aqui",
        "modules": [
            {
                "title": "1. Decomposição de Monólitos e API Gateway",
                "blocks": [
                    ("text", ["O <b>API Gateway</b> atua como ponto único de entrada para requisições externas, roteando chamadas e gerenciando autenticação e balanceamento."])
                ]
            },
            {
                "title": "2. Mensageria Assíncrona e Resiliência",
                "blocks": [
                    ("text", ["Comunicação orientada a eventos com RabbitMQ/Kafka e aplicação de <b>Circuit Breaker</b> para evitar efeito cascata em falhas."])
                ]
            }
        ]
    },
    {
        "id": 13,
        "filename": "materiais/frontend_react_nextjs.pdf",
        "title": "Desenvolvimento Frontend com React e Next.js",
        "subtitle": "React 18+, Hooks Avançados, App Router, SSR, SSG e TailwindCSS",
        "duration": "50 horas",
        "level": "Intermediário / Avançado",
        "category": "Frontend",
        "author": "Profª. Camilla Rocha — Cursa Aqui",
        "modules": [
            {
                "title": "1. React Hooks e Virtual DOM",
                "blocks": [
                    ("text", ["Gerenciamento de estado e ciclo de vida com `useState`, `useEffect`, `useMemo` e `useCallback`."])
                ]
            },
            {
                "title": "2. Next.js App Router e Server-Side Rendering (SSR)",
                "blocks": [
                    ("text", ["O <b>Server-Side Rendering (SSR)</b> renderiza o HTML completo no servidor a cada requisição, melhorando drasticamente a velocidade de abertura e a indexação de SEO no Google."]),
                    ("code", "export default async function CatalogoPage() {\n  const res = await fetch('https://api.cursaaqui.com/cursos', { cache: 'no-store' });\n  const cursos = await res.json();\n  return <div>{cursos.map(c => <h3 key={c.id}>{c.title}</h3>)}</div>;\n}")
                ]
            }
        ]
    },
    {
        "id": 14,
        "filename": "materiais/gestao_agil_scrum_kanban.pdf",
        "title": "Gestão Ágil de Projetos com Scrum e Kanban",
        "subtitle": "Manifesto Ágil, Papéis, Cerimônias e Limites de WIP no Kanban",
        "duration": "30 horas",
        "level": "Fundamental / Intermediário",
        "category": "Metodologias Ágeis",
        "author": "Prof. Rodrigo Nogueira — Cursa Aqui",
        "modules": [
            {
                "title": "1. O Framework Scrum Completo",
                "blocks": [
                    ("text", ["• <b>Papéis:</b> Product Owner (PO), Scrum Master e Developers.",
                             "• <b>Cerimônias:</b> Sprint Planning, Daily Scrum (15 min), <b>Sprint Review</b> (inspecionar o incremento com stakeholders) e Sprint Retrospective."])
                ]
            },
            {
                "title": "2. Método Kanban e Métricas de Fluxo",
                "blocks": [
                    ("text", ["Visualização do fluxo, limites de trabalho em progresso (WIP Limits), Lead Time e Cycle Time."])
                ]
            }
        ]
    },
    {
        "id": 15,
        "filename": "materiais/logica_programacao_algoritmos.pdf",
        "title": "Lógica de Programação e Algoritmos para Iniciantes",
        "subtitle": "Pensamento Computacional, Condicionais, Laços de Repetição e Funções",
        "duration": "40 horas",
        "level": "Iniciante",
        "category": "Fundamentos de TI",
        "author": "Prof. Marcelo Moreira — Cursa Aqui",
        "modules": [
            {
                "title": "1. Algoritmos e Tipos Primitivos",
                "blocks": [
                    ("text", ["Algoritmo é uma sequência finita e lógica de instruções.",
                             "Tipos primitivos: Inteiro, Float, String e Booleano."])
                ]
            },
            {
                "title": "2. Estruturas de Decisão e Repetição (Loops)",
                "blocks": [
                    ("text", ["• Estrutura Condicional: `SE / SENÃO`.",
                             "• Estrutura de Repetição: `ENQUANTO / WHILE` para executar blocos enquanto uma condição for verdadeira."]),
                    ("code", "notas = [8.5, 7.0, 9.0, 6.5]\nsoma = sum(notas)\nmedia = soma / len(notas)\nif media >= 7.0:\n    print(f'Aprovado com média {media:.1f}!')")
                ]
            }
        ]
    }
]

def build_pdf(course):
    pdf_path = course["filename"]
    doc = SimpleDocTemplate(
        pdf_path,
        pagesize=letter,
        leftMargin=40,
        rightMargin=40,
        topMargin=40,
        bottomMargin=45
    )

    story = []

    # Cabeçalho Superior
    top_header = [
        [P("🎓 <b>CURSA AQUI</b> — MANUAL ACADÊMICO E GUIA COMPLETO", "TableHead"),
         P(f"Carga: <b>{course['duration']}</b> | Nível: <b>{course['level']}</b>", "TableHead")]
    ]
    t_top = Table(top_header, colWidths=[352, 180])
    t_top.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,-1), primary),
        ('PADDING', (0,0), (-1,-1), 8),
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE'),
    ]))
    story.append(t_top)
    story.append(Spacer(1, 10))

    story.append(P(course["title"], "CoverTitle"))
    story.append(P(course["subtitle"], "CoverSubtitle"))
    story.append(P(f"<b>Docência:</b> {course['author']} | <b>Área:</b> {course['category']} | <b>Material Oficial</b>", "CoverMeta"))
    story.append(Spacer(1, 5))
    story.append(HRFlowable(width="100%", thickness=1.5, color=primary, spaceBefore=2, spaceAfter=10))

    # Sumário
    sum_items = [f"• <b>Seção {i+1}:</b> {m['title']}" for i, m in enumerate(course["modules"])]
    story.append(make_box("📚 Estrutura do Conteúdo Programático:", "<br/>".join(sum_items), bg="#EFF6FF", border="#93C5FD"))
    story.append(Spacer(1, 10))

    # Módulos
    for i, mod in enumerate(course["modules"]):
        mod_flowables = []
        mod_flowables.append(P(f"Seção {i+1}: {mod['title']}", "ModuleH1"))
        mod_flowables.append(HRFlowable(width="100%", thickness=0.8, color=colors.HexColor("#93C5FD"), spaceBefore=2, spaceAfter=6))

        for block_type, block_data in mod["blocks"]:
            if block_type == "text":
                for paragraph_text in block_data:
                    mod_flowables.append(P(paragraph_text, "Body"))
            elif block_type == "callout":
                title, text = block_data
                mod_flowables.append(Spacer(1, 3))
                mod_flowables.append(make_box(title, text))
                mod_flowables.append(Spacer(1, 4))
            elif block_type == "code":
                mod_flowables.append(Spacer(1, 3))
                mod_flowables.append(make_code(block_data))
                mod_flowables.append(Spacer(1, 4))
            elif block_type == "table":
                headers, rows = block_data
                mod_flowables.append(Spacer(1, 3))
                mod_flowables.append(make_table(headers, rows))
                mod_flowables.append(Spacer(1, 4))

        mod_flowables.append(Spacer(1, 8))
        story.append(KeepTogether(mod_flowables))

    # Guia de Certificação
    story.append(Spacer(1, 4))
    story.append(make_box(
        "🏆 Diretrizes para Obtenção do Certificado Digital:",
        "1. Estude atentamente os tópicos e códigos apresentados nesta apostila oficial.<br/>"
        "2. Retorne à plataforma <b>Cursa Aqui</b> e conclua o <b>Quiz de Avaliação</b> do curso.<br/>"
        "3. Ao atingir 100% de conclusão, seu <b>Certificado Oficial Autenticado</b> estará disponível imediatamente para emissão e impressão.",
        bg="#F0FDF4",
        border="#86EFAC"
    ))

    doc.build(story, canvasmaker=PaginatedCanvas)
    print(f"[OK] {pdf_path}")

print("Iniciando compilação dos 15 manuais oficiais...")
for c in courses_data:
    build_pdf(c)

print("Todas as apostilas foram criadas com sucesso!")
