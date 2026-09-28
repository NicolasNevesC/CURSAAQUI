// ============================================================
// utils.js - Funções utilitárias de segurança (escape/validação)
//
// Usadas em todos os pontos onde dados vindos do localStorage,
// da URL ou de input do usuário são inseridos no DOM via
// innerHTML/template strings, para evitar XSS armazenado.
// ============================================================

// Escapa caracteres especiais de HTML. Converte o valor para
// string (null/undefined viram "") antes de escapar.
function esc(value) {
  if (value === null || value === undefined) return "";
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;");
}

// Valida URLs usadas em href/src dinâmicos. Aceita apenas URLs
// relativas ou com protocolo http/https; qualquer outra coisa
// (javascript:, data:, vbscript:, etc.) retorna "#".
function safeUrl(value) {
  const str = value === null || value === undefined ? "" : String(value).trim();
  if (!str) return "#";

  // URLs relativas (não começam com um esquema tipo "algo:")
  if (!/^[a-zA-Z][a-zA-Z0-9+.-]*:/.test(str)) {
    return str;
  }

  if (/^https?:/i.test(str)) {
    return str;
  }

  return "#";
}

window.esc = esc;
window.safeUrl = safeUrl;
