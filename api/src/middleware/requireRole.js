const ApiError = require("../lib/ApiError");

// Genérico sobre o enum Role (STUDENT/TEACHER/ADMIN/SECRETARIA). Nesta etapa
// só é usado com 'STUDENT' e 'TEACHER', mas aceita qualquer combinação dos
// 4 papéis para quando rotas de ADMIN/SECRETARIA forem adicionadas.
function requireRole(...roles) {
  return function (req, res, next) {
    if (!req.user) {
      return next(new ApiError(401, "Token de autenticação ausente."));
    }
    if (!roles.includes(req.user.role)) {
      return next(new ApiError(403, "Você não tem permissão para acessar este recurso."));
    }
    next();
  };
}

module.exports = requireRole;
