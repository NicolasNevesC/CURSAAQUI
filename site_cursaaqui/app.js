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
 * MÓDULO DE CURSOS & SISTEMA DE NÍVEIS
 */
let currentLevelFilter = 'all';

function generateCourseCardHTML(course, actualProgress, user) {
  const minLevel = course.minLevel || 1;
  const userLevel = user ? (user.level || 1) : 1;
  const isTeacher = user && user.role === "teacher";
  const isLocked = !isTeacher && (userLevel < minLevel);
  const safeTitle = course.title.replace(/'/g, "\\'");

  const levelBadgeClass = minLevel === 1 ? 'lvl-1' : (minLevel === 2 ? 'lvl-2' : 'lvl-3');
  const levelBadgeLabel = minLevel === 1 ? '🌱 Nível 1 • Básico' : (minLevel === 2 ? '📘 Nível 2 • Intermediário' : '👑 Nível 3 • Avançado');
  const levelTagHTML = `<span class="card-level-pill ${levelBadgeClass}">${minLevel === 1 ? '🌱 Nível 1' : (minLevel === 2 ? '📘 Nível 2' : '👑 Nível 3')}</span>`;

  if (isLocked) {
    return `
      <div class="course-card is-locked" onclick="showLockedModal('${safeTitle}', ${minLevel}, ${userLevel})">
        <div class="course-img-wrapper">
          <img src="${course.image}" alt="${course.title}">
          <span class="category-tag">${course.category}</span>
          ${levelTagHTML}
          <span class="lock-tag">🔒 Nível ${minLevel}</span>
          <div class="lock-overlay">
            <div class="lock-center-icon">🔒</div>
          </div>
        </div>
        <div class="course-content">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;">
            <span style="font-size: 0.75rem; font-weight: 700; color: #EF4444; display: inline-flex; align-items: center; gap: 4px;">🔒 Requer Nível ${minLevel}</span>
            <span style="font-size: 0.75rem; opacity: 0.75; display: inline-flex; align-items: center; gap: 4px;">📄 Apostila em PDF</span>
          </div>
          <h3 class="course-title" title="${course.title}">${course.title}</h3>
          <div class="course-meta">
            <span>👨‍🏫 ${course.teacher}</span>
            <div class="course-stats">
              <span>⭐ ${course.rating}</span>
              <span>⏱️ ${course.duration}</span>
            </div>
          </div>
          <div class="progress">
            <div class="progress-bar" style="width: 0%"></div>
          </div>
          <button class="btn-locked card-btn" onclick="event.stopPropagation(); showLockedModal('${safeTitle}', ${minLevel}, ${userLevel})">
            🔒 Desbloqueia no Nível ${minLevel}
          </button>
        </div>
      </div>
    `;
  }

  const unlockBadge = (minLevel > 1) 
    ? `<span class="unlocked-tag">🔓 Nível ${minLevel}</span>`
    : ``;

  return `
    <div class="course-card" onclick="watchCourse('${safeTitle}')">
      <div class="course-img-wrapper">
        <img src="${course.image}" alt="${course.title}">
        <span class="category-tag">${course.category}</span>
        ${levelTagHTML}
        ${unlockBadge}
      </div>
      <div class="course-content">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;">
          <span style="font-size: 0.75rem; font-weight: 700; color: #10B981; display: inline-flex; align-items: center; gap: 4px;">🔓 Nível ${minLevel} Liberado</span>
          <span style="font-size: 0.75rem; opacity: 0.75; display: inline-flex; align-items: center; gap: 4px;">📄 Apostila em PDF</span>
        </div>
        <h3 class="course-title" title="${course.title}">${course.title}</h3>
        <div class="course-meta">
          <span>👨‍🏫 ${course.teacher}</span>
          <div class="course-stats">
            <span>⭐ ${course.rating}</span>
            <span>⏱️ ${course.duration}</span>
          </div>
        </div>
        <div class="progress">
          <div class="progress-bar" style="width:${actualProgress}%"></div>
        </div>
        <button class="btn-primary card-btn" onclick="event.stopPropagation(); watchCourse('${safeTitle}')">
          Acessar Matéria ➔
        </button>
      </div>
    </div>
  `;
}

const LEVEL_TRACKS = [
  {
    level: 1,
    title: "Nível 1 — Cursos Básicos & Iniciantes",
    desc: "Matérias essenciais, diretrizes acadêmicas, lógica de programação e fundamentos de TI.",
    badgeClass: "lvl-1",
    badgeText: "🌱 Nível 1 • Iniciante"
  },
  {
    level: 2,
    title: "Nível 2 — Cursos Intermediários",
    desc: "Desenvolvimento mobile, redes de computadores, banco de dados SQL/NoSQL, React e metodologias ágeis.",
    badgeClass: "lvl-2",
    badgeText: "📘 Nível 2 • Intermediário"
  },
  {
    level: 3,
    title: "Nível 3 — Cursos Avançados & Especialistas",
    desc: "Cloud Computing, DevOps, APIs .NET, Inteligência Artificial, Cybersecurity e Arquitetura de Microserviços.",
    badgeClass: "lvl-3",
    badgeText: "👑 Nível 3 • Avançado"
  }
];

// Estado dos accordions dos níveis (abertos por padrão)
let levelAccordionState = {
  1: true,
  2: true,
  3: true
};

function toggleLevelAccordion(level) {
  levelAccordionState[level] = !levelAccordionState[level];
  const wrapper = document.getElementById(`level-accordion-${level}`);
  if (wrapper) {
    if (levelAccordionState[level]) {
      wrapper.classList.remove('is-collapsed');
    } else {
      wrapper.classList.add('is-collapsed');
    }
  }
}

function filterByLevel(level) {
  currentLevelFilter = level === 'all' ? 'all' : Number(level);
  
  document.querySelectorAll('.level-tab-btn').forEach(btn => {
    const btnLevel = btn.getAttribute('data-level');
    if (String(btnLevel) === String(level)) {
      btn.classList.add('active');
    } else {
      btn.classList.remove('active');
    }
  });

  filterCourses();
}

function renderUserLevelBanner() {
  const banner = document.getElementById("userLevelBanner");
  if (!banner) return;

  const user = getCurrentUser();
  if (user) {
    const userLevel = user.level || 1;
    const isTeacher = user.role === "teacher";
    if (isTeacher) {
      banner.innerHTML = `
        <div class="level-banner-card teacher">
          <div class="level-banner-content">
            <span class="level-banner-badge">👨‍🏫 Perfil Professor</span>
            <div class="level-banner-text">
              <h4>Acesso Geral aos Níveis & Avaliações</h4>
              <p>Como docente, todos os níveis de formação estão destrancados para sua gestão e validação pedagógica.</p>
            </div>
          </div>
        </div>
      `;
    } else {
      const nextLevelText = userLevel < 3 
        ? `Conclua os cursos do Nível ${userLevel} para avançar ao Nível ${userLevel + 1}!` 
        : `Parabéns! Você alcançou o Nível 3 (Avançado/Especialista) com todos os cursos liberados!`;
      banner.innerHTML = `
        <div class="level-banner-card student">
          <div class="level-banner-content">
            <span class="level-banner-badge lvl-${userLevel}">⭐ Seu Progresso: Nível ${userLevel}</span>
            <div class="level-banner-text">
              <h4>Aluno: ${user.name} (${user.xp || 0} XP)</h4>
              <p>Níveis liberados para você: <strong>Nível 1 ${userLevel >= 2 ? '• Nível 2' : ''} ${userLevel >= 3 ? '• Nível 3' : ''}</strong>. ${nextLevelText}</p>
            </div>
          </div>
        </div>
      `;
    }
  } else {
    banner.innerHTML = `
      <div class="level-banner-card visitor">
        <div class="level-banner-content">
          <span class="level-banner-badge lvl-1">🌱 Nível 1 Inicial • Liberado</span>
          <div class="level-banner-text">
            <h4>Acesso Inicial de Estudante (Nível 1)</h4>
            <p>Todos os cursos do <strong>Nível 1</strong> estão livres para você estudar. Faça <a href="#" onclick="openAuthModal(); return false;" style="color:var(--primary); font-weight:700; text-decoration:underline;">login ou crie sua conta</a> para registrar notas, ganhar XP e desbloquear os <strong>Níveis 2 e 3</strong>!</p>
          </div>
        </div>
      </div>
    `;
  }
}

function renderLevelTracks(coursesList, records, user, container) {
  const userLevel = user ? (user.level || 1) : 1;
  const isTeacher = user && user.role === "teacher";

  let html = "";
  const tracksToRender = currentLevelFilter === 'all'
    ? LEVEL_TRACKS
    : LEVEL_TRACKS.filter(t => t.level === Number(currentLevelFilter));

  tracksToRender.forEach(track => {
    const trackCourses = coursesList.filter(c => (c.minLevel || 1) === track.level);
    if (trackCourses.length === 0) return;

    const isTrackLocked = !isTeacher && (userLevel < track.level);
    const isOpen = levelAccordionState[track.level] !== false;
    
    const trackStatusPill = isTrackLocked 
      ? `<span class="level-status-pill locked">🔒 Bloqueado (Requer Nível ${track.level})</span>`
      : `<span class="level-status-pill unlocked">🔓 Liberado (${trackCourses.length} ${trackCourses.length > 1 ? 'Matérias em PDF' : 'Matéria em PDF'})</span>`;

    html += `
      <div id="level-accordion-${track.level}" class="level-track-wrapper ${isTrackLocked ? 'is-locked-track' : ''} ${!isOpen ? 'is-collapsed' : ''}">
        <div class="level-track-header" onclick="toggleLevelAccordion(${track.level})" title="Clique para expandir ou recolher o Nível ${track.level}">
          <div class="level-track-info">
            <div class="level-track-tag-row">
              <span class="level-badge-pill ${track.badgeClass}">${track.badgeText}</span>
              ${isTrackLocked ? `<span class="level-locked-indicator">🔒 Requer Nível ${track.level} para Acesso</span>` : '<span class="level-unlocked-indicator" style="display:inline-flex;align-items:center;gap:4px;font-size:0.75rem;font-weight:700;color:#10B981;background:rgba(16,185,129,0.12);padding:3px 10px;border-radius:12px;border:1px solid rgba(16,185,129,0.25);">🔓 Liberado para Você</span>'}
            </div>
            <h3 class="level-track-title">${track.title}</h3>
            <p class="level-track-desc">${track.desc}</p>
          </div>
          <div class="level-track-status-box">
            ${trackStatusPill}
            <button class="accordion-toggle-btn" aria-label="Expandir ou recolher">
              <span class="accordion-arrow">▼</span>
            </button>
          </div>
        </div>
        <div class="level-track-body-wrapper">
          <div class="level-track-body">
            <div class="level-track-grid">
              ${trackCourses.map(course => {
                const actualProgress = records[course.id] ? records[course.id].progress : (course.progress || 0);
                return generateCourseCardHTML(course, actualProgress, user);
              }).join('')}
            </div>
          </div>
        </div>
      </div>
    `;
  });

  container.innerHTML = html;
  renderUserLevelBanner();
}

// Catálogo agora vem da API (populada via seed a partir do antigo courses.js).
// Cacheado em memória por página para não refazer a requisição a cada filtro.
let __coursesCache = null;

async function getAllCourses() {
  if (__coursesCache) return __coursesCache;
  try {
    __coursesCache = await apiFetch("/courses");
  } catch (e) {
    __coursesCache = [];
  }
  return __coursesCache;
}

async function renderCourses() {
  const grid = document.getElementById("courseGrid");
  if (!grid) return;

  grid.innerHTML = "";

  const user = getCurrentUser();
  const records = user ? await apiFetch("/progress").catch(() => ({})) : {};
  const coursesList = await getAllCourses();

  if (coursesList && coursesList.length > 0) {
    renderLevelTracks(coursesList, records, user, grid);
  }
}

async function watchCourse(courseName) {
  const user = getCurrentUser();
  if (!user) {
    showToast("Faça login para assistir as aulas.");
    if (typeof openAuthModal === "function") openAuthModal();
    return;
  }
  const coursesList = await getAllCourses();
  if (coursesList) {
    const found = coursesList.find(c => c.title === courseName);
    if (found) {
      const minLevel = found.minLevel || 1;
      const userLevel = user.level || 1;
      if (user.role !== "teacher" && userLevel < minLevel) {
        showLockedModal(found.title, minLevel, userLevel);
        return;
      }
      // selectedCourse continua no localStorage: é só o "handoff" transiente
      // entre o catálogo e course.html, não um dado que precise de persistência real.
      localStorage.setItem("selectedCourse", JSON.stringify(found));
    }
  }
  window.location.href = `course.html?name=${encodeURIComponent(courseName)}`;
}

function clearSearchInput() {
  const searchInput = document.getElementById("searchInput");
  if (searchInput) searchInput.value = "";
  filterByLevel('all');
}

function setupFilterEvents() {
  const searchInput = document.getElementById("searchInput");
  if (searchInput) {
    searchInput.value = "";
    setTimeout(() => {
      if (searchInput.value.includes("@")) {
        searchInput.value = "";
        renderCourses();
      }
    }, 200);
    setTimeout(() => {
      if (searchInput.value.includes("@")) {
        searchInput.value = "";
        renderCourses();
      }
    }, 800);
    searchInput.addEventListener("input", filterCourses);
  }
}

async function filterCourses() {
  const searchInput = document.getElementById("searchInput");
  const grid = document.getElementById("courseGrid");

  if (!grid) return;

  let search = searchInput ? searchInput.value.toLowerCase().trim() : "";
  if (search.includes("@")) {
    search = "";
    if (searchInput) searchInput.value = "";
  }

  const coursesList = await getAllCourses();
  if (!coursesList) return;

  const user = getCurrentUser();

  let filtered = coursesList.filter(course => {
    const matchesSearch = !search || (
      course.title.toLowerCase().includes(search) || 
      course.category.toLowerCase().includes(search) ||
      (course.area && course.area.toLowerCase().includes(search))
    );
    const courseLevel = course.minLevel || 1;
    const matchesLevel = currentLevelFilter === 'all' || courseLevel === Number(currentLevelFilter);
    return matchesSearch && matchesLevel;
  });

  grid.innerHTML = "";

  if (filtered.length === 0) {
    grid.innerHTML = `
      <div style="grid-column: 1 / -1; text-align: center; padding: 40px; background: var(--card); border-radius: 16px; border: 1px solid rgba(0,0,0,0.1); margin: 20px auto; max-width: 600px; width: 100%;">
        <h3>Nenhuma matéria encontrada</h3>
        <p style="margin: 10px 0 20px 0; opacity: 0.8;">Tente outro filtro de nível ou termo de busca.</p>
        <button class="btn-primary" onclick="clearSearchInput()">Mostrar Todos os Cursos</button>
      </div>
    `;
    return;
  }

  const records = user ? await apiFetch("/progress").catch(() => ({})) : {};
  renderLevelTracks(filtered, records, user, grid);
}

/**
 * MODAL DE CADEADO E ANIMAÇÃO DE DESTRANCAMENTO
 */
function showLockedModal(courseTitle, minLevel, currentLevel) {
  let modal = document.getElementById("lockModal");
  if (!modal) {
    modal = document.createElement("div");
    modal.id = "lockModal";
    modal.className = "modal";
    modal.style.display = "none";
    document.body.appendChild(modal);
  }

  const user = getCurrentUser();
  const userXp = user ? (user.xp || 0) : 0;
  const userLvl = user ? (user.level || 1) : (currentLevel || 1);
  const xpNeeded = userLvl * 1000;
  const progressPercent = Math.min(100, Math.round((userXp / xpNeeded) * 100));

  modal.innerHTML = `
    <div class="modal-content lock-modal-card">
      <span class="close-cert" style="position:absolute; top:15px; right:20px; cursor:pointer; font-size:1.5rem;" onclick="closeLockedModal()">&times;</span>
      
      <div class="padlock-anim-container">
        <div class="padlock-wrapper" id="modalPadlock">
          <div class="padlock-halo"></div>
          <div class="padlock-sparkles">
            <span class="sparkle s1">✨</span>
            <span class="sparkle s2">⭐</span>
            <span class="sparkle s3">✨</span>
            <span class="sparkle s4">⭐</span>
          </div>
          <svg class="padlock-svg" viewBox="0 0 100 120">
            <defs>
              <linearGradient id="shackleGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                <stop offset="0%" stop-color="#CBD5E1" />
                <stop offset="50%" stop-color="#94A3B8" />
                <stop offset="100%" stop-color="#64748B" />
              </linearGradient>
              <linearGradient id="bodyGradGold" x1="0%" y1="0%" x2="100%" y2="100%">
                <stop offset="0%" stop-color="#FBBF24" />
                <stop offset="50%" stop-color="#EAB308" />
                <stop offset="100%" stop-color="#B45309" />
              </linearGradient>
            </defs>
            <path class="padlock-shackle" d="M 32 55 V 30 A 18 18 0 0 1 68 30 V 55" fill="none" stroke="url(#shackleGrad)" stroke-width="12" stroke-linecap="round" />
            <rect class="padlock-body" id="modalPadlockBody" x="16" y="48" width="68" height="58" rx="14" fill="url(#bodyGradGold)" stroke="#92400E" stroke-width="2" />
            <circle cx="50" cy="72" r="6" fill="#1E293B" />
            <polygon points="47,73 53,73 55,87 45,87" fill="#1E293B" />
          </svg>
        </div>
      </div>

      <h2 style="font-size: 1.4rem; margin-bottom: 6px; color: var(--text);">Curso Bloqueado por Nível</h2>
      <p style="font-size: 1.05rem; font-weight: 700; color: var(--primary); margin-bottom: 12px;">${courseTitle}</p>
      
      <p style="font-size: 0.92rem; opacity: 0.85; line-height: 1.5; margin-bottom: 15px;">
        Este curso é avançado e requer <strong>Nível ${minLevel}</strong> de experiência para ser cursado.
      </p>

      <div class="level-req-box">
        <div class="level-req-row">
          <span>Seu Nível Atual: <strong>Nível ${userLvl}</strong></span>
          <span style="color: #EF4444; font-weight: 700;">Requer Nível ${minLevel}</span>
        </div>
        <div class="level-req-bar">
          <div class="level-req-fill" style="width: ${progressPercent}%;"></div>
        </div>
        <div style="display:flex; justify-content:space-between; margin-top:6px; font-size:0.8rem; opacity:0.8;">
          <span>XP Atual: ${userXp} XP</span>
          <span>Próximo Nível: ${xpNeeded} XP</span>
        </div>
      </div>

      <div style="background: rgba(59, 130, 246, 0.08); border-radius: 10px; padding: 12px; margin-bottom: 20px; font-size: 0.85rem; text-align: left;">
        💡 <strong>Como Desbloquear:</strong> Conclua as aulas e gabarite os quizzes dos cursos anteriores (Nível 1 a ${Math.max(1, minLevel - 1)}) para ganhar XP e subir de nível!
      </div>

      <div style="display: flex; gap: 10px; justify-content: center; flex-wrap: wrap;">
        <button class="btn-secondary" onclick="playPadlockUnlockDemo()" style="display: inline-flex; align-items: center; gap: 6px;">
          ✨ Testar Efeito de Desbloqueio
        </button>
        <button class="btn-primary" onclick="closeLockedModal()">
          Entendido
        </button>
      </div>
    </div>
  `;

  modal.style.display = "flex";

  const padlock = document.getElementById("modalPadlock");
  if (padlock) {
    padlock.classList.add("shake");
    setTimeout(() => padlock.classList.remove("shake"), 600);
  }
}

function closeLockedModal() {
  const modal = document.getElementById("lockModal");
  if (modal) modal.style.display = "none";
}

function playPadlockUnlockDemo() {
  const padlock = document.getElementById("modalPadlock");
  if (!padlock) return;

  padlock.classList.remove("shake");
  padlock.classList.remove("unlocked");
  void padlock.offsetWidth; // reflow

  padlock.classList.add("unlocked");
  showToast("🔓 O cadeado foi destrancado com sucesso!");

  setTimeout(() => {
    setTimeout(() => {
      if (padlock && padlock.classList.contains("unlocked")) {
        padlock.classList.remove("unlocked");
      }
    }, 3500);
  }, 100);
}

/**
 * MÓDULO DE INTERFACE DE AUTENTICAÇÃO
 */
function updateAuthUI() {
  const loginBtn = document.getElementById("login-btn");
  const userProfile = document.getElementById("user-profile");

  const user = getCurrentUser();

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

    updateUserStats();
  } else {
    if (loginBtn) loginBtn.style.display = "block";
    if (userProfile) userProfile.style.display = "none";
  }
}

async function updateUserStats() {
  const list = document.getElementById("started-courses-list");
  if (!list) return;
  list.innerHTML = "";

  const records = await apiFetch("/progress").catch(() => ({}));
  const coursesList = await getAllCourses();

  const keys = Object.keys(records);
  if (keys.length === 0) {
    list.innerHTML = `<li style="font-style: italic; opacity: 0.6;">Nenhum curso iniciado</li>`;
    return;
  }

  keys.forEach(id => {
    const record = records[id];
    const course = coursesList.find(c => c.id === Number(id));
    list.innerHTML += `
      <li>
        <span>${course ? course.title : "Curso"}</span>
        <strong>${record.progress}%</strong>
      </li>
    `;
  });
}