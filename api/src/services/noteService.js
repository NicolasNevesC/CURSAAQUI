const prisma = require("../lib/prisma");

// courseId "0" no frontend (herdado do localStorage) significa "notas gerais",
// que mapeamos para courseId null no banco (ver comentário no schema.prisma).
function normalizeCourseId(courseId) {
  const num = Number(courseId);
  return num === 0 ? null : num;
}

async function get(userId, courseId) {
  const note = await prisma.note.findFirst({
    where: { userId, courseId: normalizeCourseId(courseId) },
  });
  return note ? note.content : "";
}

async function upsert(userId, courseId, content) {
  const normalized = normalizeCourseId(courseId);
  const existing = await prisma.note.findFirst({ where: { userId, courseId: normalized } });

  if (existing) {
    return prisma.note.update({ where: { id: existing.id }, data: { content } });
  }
  return prisma.note.create({ data: { userId, courseId: normalized, content } });
}

module.exports = { get, upsert };
