const prisma = require("../lib/prisma");
const ApiError = require("../lib/ApiError");

async function listForCourse(courseId) {
  return prisma.review.findMany({
    where: { courseId: Number(courseId) },
    orderBy: { createdAt: "desc" },
    include: { user: { select: { name: true } } },
  });
}

// userId null = avaliação anônima (preserva o comportamento atual, onde um
// visitante sem login também pode enviar uma review com user "anonimo").
async function create({ courseId, userId, rating, comment }) {
  if (!rating || rating < 1 || rating > 5) {
    throw new ApiError(400, "A avaliação deve ser entre 1 e 5.");
  }
  return prisma.review.create({
    data: { courseId: Number(courseId), userId: userId || null, rating: Number(rating), comment },
  });
}

module.exports = { listForCourse, create };
