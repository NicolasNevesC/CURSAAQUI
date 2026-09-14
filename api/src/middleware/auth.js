const { verifyToken } = require("../lib/jwt");
const ApiError = require("../lib/ApiError");

// Popula req.user = { userId, role } a partir do JWT. 401 se ausente/inválido.
function requireAuth(req, res, next) {
  const header = req.headers.authorization || "";
  const [scheme, token] = header.split(" ");

  if (scheme !== "Bearer" || !token) {
    return next(new ApiError(401, "Token de autenticação ausente."));
  }

  try {
    req.user = verifyToken(token);
    next();
  } catch (err) {
    next(new ApiError(401, "Token de autenticação inválido ou expirado."));
  }
}

// Mesma validação, mas não falha se o token estiver ausente — usado em
// rotas de leitura pública que variam a resposta conforme o usuário (ex: /courses).
function optionalAuth(req, res, next) {
  const header = req.headers.authorization || "";
  const [scheme, token] = header.split(" ");

  if (scheme === "Bearer" && token) {
    try {
      req.user = verifyToken(token);
    } catch (err) {
      // token inválido em rota opcional: segue como visitante, não bloqueia
    }
  }
  next();
}

module.exports = { requireAuth, optionalAuth };
