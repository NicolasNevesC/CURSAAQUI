// app.js - Arquivo de Lógica Global Centralizado

document.addEventListener("DOMContentLoaded", () => {
  const loadingScreen = document.getElementById("loading-screen");
  if (loadingScreen) {
    setTimeout(() => {
      loadingScreen.style.display = "none";
    }, 1000);
  }

  initTheme();
  renderCourses();
  setupFilterEvents();
  updateAuthUI();
});

/**
 * SISTEMA GLOBAL DE NOTIFICAÇÕES (TOAST)
 */
function showToast(msg) {
  const container = document.getElementById("toast-container");
  if (!container) return;
  
  const existingToasts = container.querySelectorAll(".toast");
  for (let t of existingToasts) {
    if (t.innerText === msg) return;
  }

  const toast = document.createElement("div");
  toast.className = "toast";
  toast.innerText = msg;
  container.appendChild(toast);
  
  setTimeout(() => {
    toast.remove();
  }, 3000);
}

function initTheme() {
  const savedTheme = localStorage.getItem("theme") || "light";
  
  if (savedTheme === "dark") {
    document.documentElement.classList.add("dark");
  } else {
    document.documentElement.classList.remove("dark");
  }

  const themeToggleBtn = document.getElementById("theme-toggle");
  if (themeToggleBtn) {
    themeToggleBtn.addEventListener("click", () => {
      const isDark = document.documentElement.classList.toggle("dark");
      localStorage.setItem("theme", isDark ? "dark" : "light");
    });
  }
}

/**
 * MÓDULO DE CURSOS
 */
function renderCourses() {
  const grid = document.getElementById("courseGrid");
  if (!grid) return; 

  grid.innerHTML = "";

  const user = JSON.parse(localStorage.getItem("loggedUser"));
  const records = user ? (JSON.parse(localStorage.getItem(`user_courses_progress_${user.email}`)) || {}) : {};

  if (typeof courses !== "undefined") {
    courses.forEach(course => {
      const actualProgress = records[course.id] ? records[course.id].progress : (course.progress || 0);

      grid.innerHTML += `
        <div class="course-card">
          <img src="${course.image}" alt="${course.title}">
          <div class="course-content">
            <h3>${course.title}</h3>
            <p class="category-tag">${course.category}</p>
            <p>Professor: ${course.teacher}</p>
            <p>⭐ ${course.rating}</p>
            <p>⏱️ ${course.duration}</p>
            <div class="progress">
              <div class="progress-bar" style="width:${actualProgress}%"></div>
            </div>
            <button class="btn-primary" onclick="watchCourse('${course.title}')">
              Assistir
            </button>
          </div>
        </div>
      `;
    });
  }
}

function watchCourse(courseName) {
  const user = localStorage.getItem("loggedUser");
  if (!user) {
    showToast("Faça login para assistir as aulas.");
    if (typeof openAuthModal === "function") openAuthModal();
    return;
  }
  if (typeof courses !== "undefined") {
    const found = courses.find(c => c.title === courseName);
    if (found) {
      localStorage.setItem("selectedCourse", JSON.stringify(found));
    }
  }
  window.location.href = `course.html?name=${encodeURIComponent(courseName)}`;
}

/**
 * MÓDULO DE FILTROS (REMOVIDO FILTRO DE CATEGORIAS)
 */
function setupFilterEvents() {
  const searchInput = document.getElementById("searchInput");
  if (searchInput) searchInput.addEventListener("input", filterCourses);
}

function filterCourses() {
  const searchInput = document.getElementById("searchInput");
  const grid = document.getElementById("courseGrid");
  
  if (!searchInput || !grid) return;

  const search = searchInput.value.toLowerCase();

  if (typeof courses === "undefined") return;

  const filtered = courses.filter(course => {
    return course.title.toLowerCase().includes(search);
  });

  grid.innerHTML = "";

  const user = JSON.parse(localStorage.getItem("loggedUser"));
  const records = user ? (JSON.parse(localStorage.getItem(`user_courses_progress_${user.email}`)) || {}) : {};

  filtered.forEach(course => {
    const actualProgress = records[course.id] ? records[course.id].progress : (course.progress || 0);

    grid.innerHTML += `
      <div class="course-card">
        <img src="${course.image}" alt="${course.title}">
        <div class="course-content">
          <h3>${course.title}</h3>
          <p class="category-tag">${course.category}</p>
          <p>Professor: ${course.teacher}</p>
          <p>⭐ ${course.rating}</p>
          <p>⏱️ ${course.duration}</p>
          <div class="progress">
            <div class="progress-bar" style="width:${actualProgress}%"></div>
          </div>
          <button class="btn-primary" onclick="watchCourse('${course.title}')">
            Assistir
          </button>
        </div>
      </div>
    `;
  });
}

/**
 * MÓDULO DE INTERFACE DE AUTENTICAÇÃO
 */
function updateAuthUI() {
  const loginBtn = document.getElementById("login-btn");
  const userProfile = document.getElementById("user-profile");
  
  const user = JSON.parse(localStorage.getItem("loggedUser"));

  if (user) {
    if (loginBtn) loginBtn.style.display = "none";
    if (userProfile) userProfile.style.display = "block";

    if (user.xp === undefined) user.xp = 0;
    if (user.level === undefined) user.level = 1;

    const xpNeeded = user.level * 1000;
    const progressPercent = Math.min((user.xp / xpNeeded) * 100, 100);

    const menuName = document.getElementById("menu-user-name");
    const userLevel = document.getElementById("user-level-val");
    const xpRatio = document.getElementById("xp-ratio-val");
    const xpBar = document.getElementById("xp-progress-bar");

    if (menuName) menuName.innerText = user.name + (user.role === "teacher" ? " (Prof)" : "");
    if (userLevel) userLevel.innerText = user.level;
    if (xpRatio) xpRatio.innerText = `${user.xp} / ${xpNeeded} XP`;
    if (xpBar) xpBar.style.width = `${progressPercent}%`;

    updateUserStats(user.email);
  } else {
    if (loginBtn) loginBtn.style.display = "block";
    if (userProfile) userProfile.style.display = "none";
  }
}

function updateUserStats(email) {
  const list = document.getElementById("started-courses-list");
  if (!list) return;
  list.innerHTML = "";

  const progressKey = `user_courses_progress_${email}`;
  const records = JSON.parse(localStorage.getItem(progressKey)) || {};

  const keys = Object.keys(records);
  if (keys.length === 0) {
    list.innerHTML = `<li style="font-style: italic; opacity: 0.6;">Nenhum curso iniciado</li>`;
    return;
  }

  keys.forEach(id => {
    const record = records[id];
    list.innerHTML += `
      <li>
        <span>${record.title}</span>
        <strong>${record.progress}%</strong>
      </li>
    `;
  });
}

/**
 * SISTEMA DE SUPORTE TÉCNICO
 */
function enviarSuporte(event) {
  event.preventDefault();
  
  const assunto = document.getElementById("suporteAssunto").value.trim();
  const mensagem = document.getElementById("suporteMsg").value.trim();
  const user = JSON.parse(localStorage.getItem("loggedUser"));

  if (!user) {
    showToast("Você precisa estar logado para enviar um chamado.");
    openAuthModal();
    return;
  }

  const chamados = JSON.parse(localStorage.getItem("suporte_chamados")) || [];
  chamados.push({
    usuario: user.email,
    assunto: assunto,
    mensagem: mensagem,
    data: new Date().toISOString()
  });
  localStorage.setItem("suporte_chamados", JSON.stringify(chamados));
  
  document.getElementById("suporteForm").reset();

  alert("Sua mensagem foi encaminhada à equipe de suporte técnico.");
  showToast("Chamado técnico registrado!");
}