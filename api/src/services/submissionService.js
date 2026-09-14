const prisma = require("../lib/prisma");
const ApiError = require("../lib/ApiError");
const { gradeQuiz } = require("../lib/grading");

function parseSubmission(record) {
  if (!record) return null;
  return {
    ...record,
    answers: record.answersJson ? JSON.parse(record.answersJson) : {},
    answersJson: undefined,
  };
}

// Reenviar o quiz substitui a submissão anterior e volta o status para
// PENDENTE (mesmo comportamento do course.html hoje: sobrescreve o registro
// existente do mesmo aluno+curso e limpa o parecer anterior). Nota e
// aprovação são recalculadas do gabarito do curso — o score enviado pelo
// cliente é ignorado, para não dar brecha de auto-atribuir nota.
async function createOrReplace(userId, { courseId, answers, openAnswer }) {
  const course = await prisma.course.findUnique({ where: { id: Number(courseId) } });
  if (!course) {
    throw new ApiError(404, "Curso não encontrado.");
  }
  const quiz = JSON.parse(course.quizJson);
  const grade = gradeQuiz(quiz, answers || {});

  const record = await prisma.submission.upsert({
    where: { userId_courseId: { userId, courseId: Number(courseId) } },
    update: {
      answersJson: JSON.stringify(answers || {}),
      openAnswer: openAnswer || "",
      score: grade.score,
      totalQuestions: grade.totalQuestions,
      passed: grade.passed,
      status: "PENDENTE",
      feedback: null,
      submittedAt: new Date(),
      gradedAt: null,
    },
    create: {
      userId,
      courseId: Number(courseId),
      answersJson: JSON.stringify(answers || {}),
      openAnswer: openAnswer || "",
      score: grade.score,
      totalQuestions: grade.totalQuestions,
      passed: grade.passed,
    },
  });
  return parseSubmission(record);
}

async function listForTeacher({ status, courseId } = {}) {
  const records = await prisma.submission.findMany({
    where: {
      ...(status ? { status } : {}),
      ...(courseId ? { courseId: Number(courseId) } : {}),
    },
    include: {
      user: { select: { id: true, name: true, email: true } },
      course: { select: { id: true, title: true } },
    },
    orderBy: { submittedAt: "desc" },
  });
  return records.map(parseSubmission);
}

async function listMine(userId, { courseId } = {}) {
  const records = await prisma.submission.findMany({
    where: { userId, ...(courseId ? { courseId: Number(courseId) } : {}) },
    include: { course: { select: { id: true, title: true } } },
  });
  return records.map(parseSubmission);
}

async function correct(id, { feedback, status }) {
  const existing = await prisma.submission.findUnique({ where: { id: Number(id) } });
  if (!existing) {
    throw new ApiError(404, "Submissão não encontrada.");
  }

  const record = await prisma.submission.update({
    where: { id: Number(id) },
    data: {
      feedback: feedback || "Atividade avaliada e aprovada pelo professor.",
      status: status || "CORRIGIDO",
      gradedAt: new Date(),
    },
  });
  return parseSubmission(record);
}

module.exports = { createOrReplace, listForTeacher, listMine, correct };
