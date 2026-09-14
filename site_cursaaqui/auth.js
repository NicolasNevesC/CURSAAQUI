// ============================================================
// auth.js - Gerenciador de Autenticação e Modais
// Responsável por: login, logout, cadastro e controle de modal.
// Os dados de usuário agora vivem na API (Express + Prisma +
// SQL Server) via api.js — nada de usuário/senha fica mais no
// localStorage, exceto o token JWT e um cache do usuário logado
// para a UI não precisar refazer /auth/me a cada render.
// ============================================================


// Normaliza o role vindo da API (enum "STUDENT"/"TEACHER") para o
// formato lowercase ("student"/"teacher") que o resto do app já espera,
// evitando reescrever todos os `user.role === "teacher"` espalhados pelo site.
function normalizeUserRole(user) {
  if (user && typeof user.role === "string") {
    user.role = user.role.toLowerCase();
  }
  return user;
}


// ─────────────────────────────────────────────────────────────
// loginAsRole(email, password)
// Login rápido via botões de demonstração na tela de login.
// ─────────────────────────────────────────────────────────────
async function loginAsRole(email, password) {
  try {
    const { token, user } = await apiFetch("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });

    setSession(token, normalizeUserRole(user));

    if (typeof closeAuthModal === "function") closeAuthModal();

    if (user.role === "teacher") {
      window.location.href = "professor.html";
      return;
    }

    if (typeof showToast === "function") showToast("Login realizado com sucesso!");
    if (typeof updateAuthUI === "function") updateAuthUI();
    if (typeof renderCourses === "function") renderCourses();
    if (typeof loadDashboard === "function") loadDashboard();
  } catch (err) {
    // apiFetch já disparou o showToast com a mensagem de erro da API
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
  document.getElementById("loginForm").style.display = "block";
  document.getElementById("registerForm").style.display = "none";
}

function showRegister(){
  document.getElementById("loginForm").style.display = "none";
  document.getElementById("registerForm").style.display = "block";
}


// ─────────────────────────────────────────────────────────────
// register()
// Lê os campos do formulário de cadastro e chama POST /auth/register.
// ─────────────────────────────────────────────────────────────
async function register(){
  const name     = document.getElementById("registerName").value.trim();
  const email    = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value;
  const role     = document.getElementById("registerRole").value; // 'student' ou 'teacher'

  if(!name || !email || !password) {
    if (typeof showToast === "function") showToast("Preencha todos os campos!");
    return;
  }

  try {
    await apiFetch("/auth/register", {
      method: "POST",
      body: JSON.stringify({ name, email, password, role }),
    });

    if (typeof showToast === "function") showToast("Conta criada com sucesso!");
    showLogin();
  } catch (err) {
    // apiFetch já mostrou o toast de erro (ex: e-mail já cadastrado)
  }
}


// ─────────────────────────────────────────────────────────────
// login()
// Lê e-mail e senha dos inputs do formulário e chama POST /auth/login.
// ─────────────────────────────────────────────────────────────
async function login(){
  const emailInput    = document.getElementById("loginEmail");
  const passwordInput = document.getElementById("loginPassword");

  if (!emailInput || !passwordInput) return;

  const email    = emailInput.value.trim();
  const password = passwordInput.value;

  try {
    const { token, user } = await apiFetch("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });

    setSession(token, normalizeUserRole(user));

    if (typeof showToast === "function") {
      showToast("Login realizado com sucesso!");
    }

    closeAuthModal();

    if (user.role === "teacher") {
      window.location.href = "professor.html";
      return;
    }

    if (typeof updateAuthUI === "function") updateAuthUI();
    if (typeof renderCourses === "function") renderCourses();
    if (typeof loadDashboard === "function") loadDashboard();
  } catch (err) {
    // apiFetch já mostrou o toast de erro (ex: usuário/senha incorretos)
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
// ─────────────────────────────────────────────────────────────
function logout() {
  clearSession();

  if (typeof updateAuthUI === "function") {
    updateAuthUI();
  }

  if (typeof showToast === "function") {
    showToast("Sessão encerrada!");
  }

  window.location.href = window.location.pathname;
}
