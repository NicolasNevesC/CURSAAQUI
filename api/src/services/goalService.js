const prisma = require("../lib/prisma");
const ApiError = require("../lib/ApiError");

async function list(userId) {
  return prisma.goal.findMany({ where: { userId }, orderBy: { id: "asc" } });
}

async function create(userId, text) {
  if (!text || !text.trim()) {
    throw new ApiError(400, "O texto da meta é obrigatório.");
  }
  return prisma.goal.create({ data: { userId, text: text.trim(), completed: false } });
}

async function update(userId, id, { text, completed }) {
  const goal = await prisma.goal.findUnique({ where: { id: Number(id) } });
  if (!goal || goal.userId !== userId) {
    throw new ApiError(404, "Meta não encontrada.");
  }
  return prisma.goal.update({
    where: { id: Number(id) },
    data: {
      ...(text !== undefined ? { text } : {}),
      ...(completed !== undefined ? { completed } : {}),
    },
  });
}

async function remove(userId, id) {
  const goal = await prisma.goal.findUnique({ where: { id: Number(id) } });
  if (!goal || goal.userId !== userId) {
    throw new ApiError(404, "Meta não encontrada.");
  }
  await prisma.goal.delete({ where: { id: Number(id) } });
}

module.exports = { list, create, update, remove };
