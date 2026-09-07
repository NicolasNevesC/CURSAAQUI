// ============================================================
// auth.js - Gerenciador de Autenticação e Modais
// Responsável por: login, logout, cadastro, controle de modal
// e inicialização de dados de demonstração no localStorage.
// ============================================================


// ─────────────────────────────────────────────────────────────
// IIFE (função auto-executável) que roda logo ao carregar a página.
// Garante que contas de demonstração (professor e aluno) sempre
// existam no localStorage, além de popular envios de exemplo.
// ─────────────────────────────────────────────────────────────
(function inicializarDadosProfessor() {

  // Lê a lista de usuários salva; se não existir, começa com array vazio
  const users = JSON.parse(localStorage.getItem("users")) || [];
  let updated = false; // flag: indica se precisamos salvar de volta

  // Se a conta do professor demo não existe, cria ela
  if (!users.some(u => u.email === "ricardo@cursaaqui.com")) {
    users.push({
      name: "Prof. Ricardo Silva",
      email: "ricardo@cursaaqui.com",
      password: "123456",
      role: "teacher", // professor tem acesso ao painel de correção
      xp: 1500,
      level: 5
    });
    updated = true;
  }

  // Se a conta do aluno demo não existe, cria ela
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

  // Só grava se algo foi adicionado (evita writes desnecessários)
  if (updated) {
    localStorage.setItem("users", JSON.stringify(users));
  }

  // Inicializa progresso de demonstração para o aluno demo caso não exista
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

  // ─── Envios de atividades de demonstração ───────────────────
  // Popula apenas se não houver nenhum envio salvo ainda
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
        status: "Pendente",   // aguardando correção do professor
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
        status: "Corrigido",  // já avaliado pelo professor
        feedback: "Excelente compreensão da arquitetura ASP.NET e Entity Framework! Parabéns.",
        submittedAt: "29/08/2026 18:10"
      }
    ];
    localStorage.setItem("teacher_activity_submissions", JSON.stringify(demoSubmissions));
  }
})();


// ─────────────────────────────────────────────────────────────
// loginAsRole(email, password)
// Login rápido via botões de demonstração na tela de login.
// Busca o usuário direto no localStorage, sem depender dos
// campos do formulário HTML.
// ─────────────────────────────────────────────────────────────
function loginAsRole(email, password) {
  const users = JSON.parse(localStorage.getItem("users")) || [];

  // Procura um usuário que tenha exatamente esse e-mail E senha
  const user = users.find(u => u.email === email && u.password === password);

  if (user) {
    // Salva o usuário logado para persistir a sessão
    localStorage.setItem("loggedUser", JSON.stringify(user));

    // Fecha o modal de autenticação se estiver aberto
    if (typeof closeAuthModal === "function") closeAuthModal();

    // Professor vai direto para o painel exclusivo dele
    if (user.role === "teacher") {
      window.location.href = "professor.html";
      return;
    }

    // Aluno: notifica, atualiza a UI e recarrega cursos/dashboard
    if (typeof showToast === "function") showToast("Login realizado com sucesso!");
    if (typeof updateAuthUI === "function") updateAuthUI();
    if (typeof renderCourses === "function") renderCourses();
    if (typeof loadDashboard === "function") loadDashboard();
  } else {
    // Credencial não encontrada no localStorage
    if (typeof showToast === "function") showToast("Erro ao fazer login rápido. Tente manualmente.");
  }
}


// ─────────────────────────────────────────────────────────────
// openAuthModal() / closeAuthModal()
// Exibe ou oculta o modal de login/cadastro centralizado na tela.
// ─────────────────────────────────────────────────────────────
function openAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "flex"; // "flex" para funcionar com centralização via CSS
}

function closeAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "none";
}


// ─────────────────────────────────────────────────────────────
// showLogin() / showRegister()
// Alterna entre o formulário de login e o de cadastro dentro
// do mesmo modal de autenticação (só um fica visível por vez).
// ─────────────────────────────────────────────────────────────
function showLogin(){
  document.getElementById("loginForm").style.display = "block";
  document.getElementById("registerForm").style.display = "none";
}

function showRegister(){
  document.getElementById("loginForm").style.display = "none";
  document.getElementById("registerForm").style.display = "block";
}


// ─────────────────────────────────────────────────────────────
// register()
// Lê os campos do formulário de cadastro, valida os dados e
// salva o novo usuário no localStorage.
// Impede cadastro com e-mail duplicado.
// ─────────────────────────────────────────────────────────────
function register(){
  // Lê e limpa espaços extras dos campos do formulário
  const name     = document.getElementById("registerName").value.trim();
  const email    = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value;
  const role     = document.getElementById("registerRole").value; // 'student' ou 'teacher'

  // Valida: todos os campos são obrigatórios
  if(!name || !email || !password) {
    if (typeof showToast === "function") showToast("Preencha todos os campos!");
    return;
  }

  const users = JSON.parse(localStorage.getItem("users")) || [];
  
  // Verifica se o e-mail já está em uso por outra conta
  if(users.some(u => u.email === email)) {
    if (typeof showToast === "function") showToast("Este e-mail já está cadastrado.");
    return;
  }

  // Cria o objeto do novo usuário e insere na lista
  users.push({
    name,
    email,
    password,
    role, // Será salvo implicitamente como 'student' pelo campo oculto do HTML
    xp: 0,     // começa sem experiência
    level: 1   // começa no nível 1
  });

  localStorage.setItem("users", JSON.stringify(users));

  if (typeof showToast === "function") showToast("Conta criada com sucesso!");
  showLogin(); // após cadastro, volta para a tela de login
}


// ─────────────────────────────────────────────────────────────
// login()
// Lê e-mail e senha dos inputs do formulário, autentica o
// usuário e direciona conforme o perfil:
//   - professor → professor.html
//   - aluno → permanece na página com UI atualizada
// ─────────────────────────────────────────────────────────────
function login(){
  const emailInput    = document.getElementById("loginEmail");
  const passwordInput = document.getElementById("loginPassword");

  // Sai silenciosamente se os elementos não existirem na página atual
  if (!emailInput || !passwordInput) return;

  const email    = emailInput.value.trim();
  const password = passwordInput.value;

  const users = JSON.parse(localStorage.getItem("users")) || [];

  // Busca o usuário com e-mail E senha correspondentes
  const user = users.find(u => u.email === email && u.password === password);

  if(user){
    // Persiste a sessão do usuário autenticado no localStorage
    localStorage.setItem("loggedUser", JSON.stringify(user));

    if (typeof showToast === "function") {
      showToast("Login realizado com sucesso!");
    }
    
    closeAuthModal();
    
    // Redirecionar professor para painel exclusivo e encerrar função
    if (user.role === "teacher") {
      window.location.href = "professor.html";
      return;
    }

    // Aluno: atualiza os componentes visuais sem redirecionar
    if (typeof updateAuthUI === "function") {
      updateAuthUI();
    }

    if (typeof renderCourses === "function") {
      renderCourses();
    }

    if (typeof loadDashboard === "function") {
      loadDashboard();
    }

  } else {
    // Nenhum usuário encontrado com essas credenciais
    if (typeof showToast === "function") {
      showToast("Usuário ou senha incorretos.");
    }
  }
}


// ─────────────────────────────────────────────────────────────
// toggleUserMenu()
// Abre ou fecha o dropdown de perfil do usuário logado
// (menu com XP, cursos iniciados, certificados e logout).
// ─────────────────────────────────────────────────────────────
function toggleUserMenu() {
  const menu = document.getElementById("user-menu");
  if (!menu) return;

  // Se estiver visível, oculta; se estiver oculto, mostra
  menu.style.display = (menu.style.display === "flex") ? "none" : "flex";
}


// ─────────────────────────────────────────────────────────────
// logout()
// Encerra a sessão do usuário removendo os dados do localStorage
// e recarregando a página na URL atual (sem hash/parâmetros).
// ─────────────────────────────────────────────────────────────
function logout() {
  localStorage.removeItem("loggedUser"); // remove a sessão salva

  // Volta a UI para o estado de "não logado"
  if (typeof updateAuthUI === "function") {
    updateAuthUI();
  }
  
  if (typeof showToast === "function") {
    showToast("Sessão encerrada!");
  }
  
  // Recarrega a mesma página sem parâmetros de rota
  window.location.href = window.location.pathname;
}