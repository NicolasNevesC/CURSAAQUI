const { PrismaClient } = require("@prisma/client");

// Singleton do PrismaClient — evita esgotar conexões com o SQL Server
// quando o nodemon recarrega o módulo em desenvolvimento.
const prisma = global.__prisma || new PrismaClient();

if (process.env.NODE_ENV !== "production") {
  global.__prisma = prisma;
}

module.exports = prisma;
