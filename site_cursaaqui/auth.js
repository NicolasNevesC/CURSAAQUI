// ============================================================
// auth.js - Gerenciador de Autenticação, Rotas e Sessões
// Responsável por: login, logout, cadastro, controle de rotas
// e inicialização de dados de demonstração no localStorage.
// ============================================================

// ─────────────────────────────────────────────────────────────
// IIFE (função auto-executável) que roda logo ao carregar a página.
// Garante que contas de demonstração (professor e aluno) sempre
// existam no localStorage, além de popular envios de exemplo.
// ─────────────────────────────────────────────────────────────
(function inicializarDadosProfessor() {
  const users = Storage.getUsers();
  let updated = false;

  if (!users.some(u => u.email === "ricardo@cursaaqui.com")) {
    users.push({
      name: "Prof. Ricardo Silva",
      email: "ricardo@cursaaqui.com",
      password: "123456",
      role: "teacher",
      xp: 1500,
      level: 5
    });
    updated = true;
  }

  if (!users.some(u => u.email === "aluno@cursaaqui.com")) {
    users.push({
      name: "Matheus Narvaes",
      email: "aluno@cursaaqui.com",
      password: "123456",
      role: "student",
      xp: 350,
      level: 2
    });
    updated = true;
  }

  if (updated) {
    Storage.setUsers(users);
  }

  const demoStudentProgressKey = "user_courses_progress_aluno@cursaaqui.com";
  if (!localStorage.getItem(demoStudentProgressKey)) {
    const demoProgress = {
      1: {
        title: "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
        progress: 85,
        pdf: "materiais/ads_manual_academico.pdf"
      },
      2: {
        title: "CLOUD COMPUTING E DEVOPS",
        progress: 100,
        pdf: "materiais/cloud_computing_devops.pdf"
      },
      3: {
        title: "SEGURANÇA DA INFORMAÇÃO E LGPD",
        progress: 40,
        pdf: "materiais/seguranca_lgpd.pdf"
      }
    };
    localStorage.setItem(demoStudentProgressKey, JSON.stringify(demoProgress));
  }

  const submissions = JSON.parse(localStorage.getItem("teacher_activity_submissions")) || [];
  if (submissions.length === 0) {
    const demoSubmissions = [
      {
        id: 101,
        userName: "Matheus Narvaes",
        userEmail: "aluno@cursaaqui.com",
        courseId: 1,
        courseTitle: "Análise e Desenvolvimento de Sistemas - Manual Acadêmico",
        answers: { 0: 2, 1: 1, 2: 1, 3: 2, 4: 1, 5: 1, 6: 3, 7: 2, 8: 2, 9: 0 },
        score: 10,
        totalQuestions: 10,
        status: "Pendente",
        feedback: "",
        submittedAt: "30/08/2026 14:20"
      },
      {
        id: 102,
        userName: "Ana Clara Souza",
        userEmail: "anaclara@gmail.com",
        courseId: 2,
        courseTitle: "CLOUD COMPUTING E DEVOPS",
        answers: { 0: 1 },
        score: 1,
        totalQuestions: 1,
        status: "Pendente",
        feedback: "",
        submittedAt: "30/08/2026 15:45"
      },
      {
        id: 103,
        userName: "Lucas Mendonça",
        userEmail: "lucas.dev@hotmail.com",
        courseId: 4,
        courseTitle: "DESENVOLVIMENTO WEB COM .NET",
        answers: { 0: 1 },
        score: 1,
        totalQuestions: 1,
        status: "Corrigido",
        feedback: "Excelente compreensão da arquitetura ASP.NET e Entity Framework! Parabéns.",
        submittedAt: "29/08/2026 18:10"
      }
    ];
    localStorage.setItem("teacher_activity_submissions", JSON.stringify(demoSubmissions));
  }
})();

// ─────────────────────────────────────────────────────────────
// redirectByRole(user)
// Redireciona o usuário para o painel correspondente ao seu perfil.
// ─────────────────────────────────────────────────────────────
function redirectByRole(user) {
  if (!user) return;
  if (user.role === "teacher" || user.role === "Professor") {
    window.location.href = "professor.html";
  } else if (user.role === "student" || user.role === "Aluno") {
    window.location.href = "aluno.html";
  } else {
    window.location.href = "aluno.html";
  }
}

// ─────────────────────────────────────────────────────────────
// isSessionValid(user)
// Valida se o usuário tem sessão ativa e se o token não expirou.
// ─────────────────────────────────────────────────────────────
function isSessionValid(user) {
  if (!user || !user.email) return false;
  if (user.tokenExpiresAt && new Date(user.tokenExpiresAt).getTime() < Date.now()) {
    Storage.clearLoggedUser();
    return false;
  }
  return true;
}

// ─────────────────────────────────────────────────────────────
// protectStudentPage()
// Protege a tela aluno.html.
// - Sem sessão válida -> redireciona para index.html
// - Professor tentando acessar -> redireciona para professor.html
// ─────────────────────────────────────────────────────────────
function protectStudentPage() {
  const user = Storage.getLoggedUser();
  if (!user || !isSessionValid(user)) {
    Storage.clearLoggedUser();
    window.location.replace("index.html?auth=required");
    return null;
  }
  if (user.role === "teacher" || user.role === "Professor") {
    window.location.replace("professor.html");
    return null;
  }
  return user;
}

// ─────────────────────────────────────────────────────────────
// protectTeacherPage()
// Protege a tela professor.html.
// - Sem sessão válida -> redireciona para index.html
// - Aluno tentando acessar -> redireciona para aluno.html
// ─────────────────────────────────────────────────────────────
function protectTeacherPage() {
  const user = Storage.getLoggedUser();
  if (!user || !isSessionValid(user)) {
    Storage.clearLoggedUser();
    window.location.replace("index.html?auth=required");
    return null;
  }
  if (user.role === "student" || user.role === "Aluno") {
    window.location.replace("aluno.html");
    return null;
  }
  return user;
}

// ─────────────────────────────────────────────────────────────
// redirectIfAlreadyAuthenticated()
// Chamado na página inicial (index.html) para encaminhar usuários já
// autenticados ao painel do seu perfil, evitando loops no logout.
// ─────────────────────────────────────────────────────────────
function redirectIfAlreadyAuthenticated() {
  const params = new URLSearchParams(window.location.search);
  if (params.get("action") === "logout") {
    Storage.clearLoggedUser();
    return;
  }
  const user = Storage.getLoggedUser();
  if (user && isSessionValid(user)) {
    redirectByRole(user);
  }
}

// ─────────────────────────────────────────────────────────────
// loginAsRole(email, password)
// Login rápido via botões de demonstração na tela de login.
// ─────────────────────────────────────────────────────────────
function loginAsRole(email, password) {
  const users = Storage.getUsers();
  const user = users.find(u => u.email === email && u.password === password);

  if (user) {
    Storage.setLoggedUser(user);

    if (typeof closeAuthModal === "function") closeAuthModal();
    if (typeof showToast === "function") showToast("Login realizado com sucesso!");

    // Redireciona conforme perfil esperado
    redirectByRole(user);
  } else {
    if (typeof showToast === "function") showToast("Erro ao fazer login rápido. Tente manualmente.");
  }
}

// ─────────────────────────────────────────────────────────────
// openAuthModal() / closeAuthModal()
// ─────────────────────────────────────────────────────────────
function openAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "flex";
}

function closeAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "none";
}

// ─────────────────────────────────────────────────────────────
// showLogin() / showRegister()
// ─────────────────────────────────────────────────────────────
function showLogin(){
  const l = document.getElementById("loginForm");
  const r = document.getElementById("registerForm");
  if (l) l.style.display = "block";
  if (r) r.style.display = "none";
}

function showRegister(){
  const l = document.getElementById("loginForm");
  const r = document.getElementById("registerForm");
  if (l) l.style.display = "none";
  if (r) r.style.display = "block";
}

// ─────────────────────────────────────────────────────────────
// register()
// ─────────────────────────────────────────────────────────────
function register(){
  const nameEl     = document.getElementById("registerName");
  const emailEl    = document.getElementById("registerEmail");
  const passwordEl = document.getElementById("registerPassword");
  const roleEl     = document.getElementById("registerRole");

  if (!nameEl || !emailEl || !passwordEl) return;

  const name     = nameEl.value.trim();
  const email    = emailEl.value.trim();
  const password = passwordEl.value;
  const role     = roleEl ? roleEl.value : "student";

  if (!name || !email || !password) {
    if (typeof showToast === "function") showToast("Preencha todos os campos!");
    return;
  }

  const users = Storage.getUsers();
  if (users.some(u => u.email === email)) {
    if (typeof showToast === "function") showToast("Este e-mail já está cadastrado.");
    return;
  }

  Storage.upsertUser({
    name,
    email,
    password,
    role: role || "student",
    xp: 0,
    level: 1
  });

  if (typeof showToast === "function") showToast("Conta criada com sucesso!");
  showLogin();
}

// ─────────────────────────────────────────────────────────────
// login()
// Lê dados do formulário, valida e redireciona:
//   - Aluno -> aluno.html
//   - Professor -> professor.html
// ─────────────────────────────────────────────────────────────
function login(){
  const emailInput    = document.getElementById("loginEmail");
  const passwordInput = document.getElementById("loginPassword");

  if (!emailInput || !passwordInput) return;

  const email    = emailInput.value.trim();
  const password = passwordInput.value;

  const users = Storage.getUsers();
  const user = users.find(u => u.email === email && u.password === password);

  if (user) {
    Storage.setLoggedUser(user);

    if (typeof showToast === "function") {
      showToast("Login realizado com sucesso!");
    }
    
    closeAuthModal();

    // Redireciona conforme o perfil
    redirectByRole(user);
  } else {
    if (typeof showToast === "function") {
      showToast("Usuário ou senha incorretos.");
    }
  }
}

// ─────────────────────────────────────────────────────────────
// toggleUserMenu()
// ─────────────────────────────────────────────────────────────
function toggleUserMenu() {
  const menu = document.getElementById("user-menu");
  if (!menu) return;
  menu.style.display = (menu.style.display === "flex") ? "none" : "flex";
}

// ─────────────────────────────────────────────────────────────
// logout()
// Encerra a sessão e redireciona de forma limpa para index.html
// ─────────────────────────────────────────────────────────────
function logout() {
  Storage.clearLoggedUser();

  if (typeof updateAuthUI === "function") {
    updateAuthUI();
  }
  
  if (typeof showToast === "function") {
    showToast("Sessão encerrada!");
  }
  
  window.location.href = "index.html?action=logout";
}
