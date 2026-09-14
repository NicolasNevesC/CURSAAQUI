const prisma = require("../lib/prisma");
const ApiError = require("../lib/ApiError");

// Os campos de conteúdo livre (lições, quiz, questão dissertativa) são
// guardados como NVARCHAR(MAX) no SQL Server (o connector sqlserver do
// Prisma não suporta o tipo Json nativo) — desserializa antes de responder.
// Expõe "teacher" (não "teacherName") e os campos de lições/quiz já
// desserializados, para bater exatamente com o shape que o frontend
// (app.js, course.html, etc.) já espera do antigo courses.js.
function parseCourse(course) {
  return {
    ...course,
    teacher: course.teacherName,
    lessons: JSON.parse(course.lessonsJson),
    quiz: JSON.parse(course.quizJson),
    openQuestion: course.openQuestionJson ? JSON.parse(course.openQuestionJson) : null,
    teacherName: undefined,
    lessonsJson: undefined,
    quizJson: undefined,
    openQuestionJson: undefined,
  };
}

async function listCourses({ category, area } = {}) {
  const courses = await prisma.course.findMany({
    where: {
      ...(category ? { category } : {}),
      ...(area ? { area } : {}),
    },
    orderBy: { id: "asc" },
  });
  return courses.map(parseCourse);
}

async function getCourseById(id) {
  const course = await prisma.course.findUnique({ where: { id: Number(id) } });
  if (!course) {
    throw new ApiError(404, "Curso não encontrado.");
  }
  return parseCourse(course);
}

module.exports = { listCourses, getCourseById, parseCourse };
