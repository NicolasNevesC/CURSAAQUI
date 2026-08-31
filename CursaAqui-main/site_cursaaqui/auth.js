// auth.js - Gerenciador de Autenticação e Modais

// Executa imediatamente para garantir que o Professor Ricardo exista no sistema
(function inicializarProfessores() {
  const users = JSON.parse(localStorage.getItem("users")) || [];
  const professorExiste = users.some(u => u.email === "ricardo@cursaaqui.com");
  
  if (!professorExiste) {
    users.push({
      name: "Ricardo",
      email: "ricardo@cursaaqui.com",
      password: "123456",
      role: "teacher",
      xp: 0,
      level: 1
    });
    localStorage.setItem("users", JSON.stringify(users));
  }
})();

function openAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "flex";
}

function closeAuthModal(){
  const modal = document.getElementById("authModal");
  if (modal) modal.style.display = "none";
}

function showLogin(){
  document.getElementById("loginForm").style.display = "block";
  document.getElementById("registerForm").style.display = "none";
}

function showRegister(){
  document.getElementById("loginForm").style.display = "none";
  document.getElementById("registerForm").style.display = "block";
}

function register(){
  const name = document.getElementById("registerName").value.trim();
  const email = document.getElementById("registerEmail").value.trim();
  const password = document.getElementById("registerPassword").value;
  const role = document.getElementById("registerRole").value;

  if(!name || !email || !password) {
    if (typeof showToast === "function") showToast("Preencha todos os campos!");
    return;
  }

  const users = JSON.parse(localStorage.getItem("users")) || [];
  
  if(users.some(u => u.email === email)) {
    if (typeof showToast === "function") showToast("Este e-mail já está cadastrado.");
    return;
  }

  users.push({
    name,
    email,
    password,
    role, // Será salvo implicitamente como 'student' pelo campo oculto do HTML
    xp: 0,
    level: 1
  });

  localStorage.setItem("users", JSON.stringify(users));

  if (typeof showToast === "function") showToast("Conta criada com sucesso!");
  showLogin();
}

function login(){
  const emailInput = document.getElementById("loginEmail");
  const passwordInput = document.getElementById("loginPassword");

  if (!emailInput || !passwordInput) return;

  const email = emailInput.value.trim();
  const password = passwordInput.value;

  const users = JSON.parse(localStorage.getItem("users")) || [];
  const user = users.find(u => u.email === email && u.password === password);

  if(user){
    localStorage.setItem("loggedUser", JSON.stringify(user));

    if (typeof showToast === "function") {
      showToast("Login realizado com sucesso!");
    }
    
    closeAuthModal();
    
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
    if (typeof showToast === "function") {
      showToast("Usuário ou senha incorretos.");
    }
  }
}

function toggleUserMenu() {
  const menu = document.getElementById("user-menu");
  if (!menu) return;
  menu.style.display = (menu.style.display === "flex") ? "none" : "flex";
}

function logout() {
  localStorage.removeItem("loggedUser");
  
  if (typeof updateAuthUI === "function") {
    updateAuthUI();
  }
  
  if (typeof showToast === "function") {
    showToast("Sessão encerrada!");
  }
  
  window.location.href = window.location.pathname;
}