const { Prisma } = require("@prisma/client");

// eslint-disable-next-line no-unused-vars
function errorHandler(err, req, res, next) {
  if (err instanceof Prisma.PrismaClientKnownRequestError && err.code === "P2002") {
    return res.status(409).json({ message: "Já existe um registro com esses dados." });
  }

  const statusCode = err.statusCode || 500;
  if (statusCode === 500) {
    console.error(err);
  }

  res.status(statusCode).json({ message: err.message || "Erro interno do servidor." });
}

module.exports = errorHandler;
