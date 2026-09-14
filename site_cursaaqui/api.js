// ============================================================
// api.js - Cliente HTTP para a API do Cursa Aqui
// Substitui o antigo acesso direto ao localStorage por chamadas
// fetch() à API Express+Prisma. Deve ser carregado ANTES de
// auth.js, app.js, dashboard.js e de qualquer <script> inline
// em course.html/aluno.html/professor.html que use apiFetch().
// ============================================================

const API_BASE = "http://localhost:3001";

function getToken() {
  return localStorage.getItem("token");
}

function setSession(token, user) {
  localStorage.setItem("token", token);
  localStorage.setItem("loggedUser", JSON.stringify(user));
}

function clearSession() {
  localStorage.removeItem("token");
  localStorage.removeItem("loggedUser");
}

function getCurrentUser() {
  try {
    return JSON.parse(localStorage.getItem("loggedUser"));
  } catch (e) {
    return null;
  }
}

// Wrapper fino sobre fetch(): injeta o token JWT, serializa/desserializa
// JSON e mostra o erro via showToast() (já usado em todo o app).
async function apiFetch(path, options = {}) {
  const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
  const token = getToken();
  if (token) headers.Authorization = `Bearer ${token}`;

  let res;
  try {
    res = await fetch(`${API_BASE}${path}`, { ...options, headers });
  } catch (networkErr) {
    if (typeof showToast === "function") {
      showToast("Não foi possível conectar à API. Verifique se o servidor está rodando.");
    }
    throw networkErr;
  }

  if (res.status === 204) return null;

  const contentType = res.headers.get("content-type") || "";
  const body = contentType.includes("application/json") ? await res.json().catch(() => ({})) : null;

  if (!res.ok) {
    const message = (body && body.message) || res.statusText;
    if (res.status === 401) clearSession();
    if (typeof showToast === "function") showToast(message);
    throw new Error(message);
  }

  return body;
}

window.apiFetch = apiFetch;
window.getToken = getToken;
window.setSession = setSession;
window.clearSession = clearSession;
window.getCurrentUser = getCurrentUser;
