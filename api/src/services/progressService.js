const prisma = require("../lib/prisma");
const { toPublicUser } = require("./authService");
const { applyLevelUps } = require("../lib/xp");
const { gradeQuiz } = require("../lib/grading");

const XP_PER_COURSE_COMPLETION = 1000;

function parseProgress(record) {
  if (!record) return null;
  return {
    ...record,
    answers: record.answersJson ? JSON.parse(record.answersJson) : {},
    answersJson: undefined,
  };
}

async function getAllForUser(userId) {
  const records = await prisma.courseProgress.findMany({ where: { userId } });
  const map = {};
  records.forEach((record) => {
    map[record.courseId] = parseProgress(record);
  });
  return map;
}

async function getOne(userId, courseId) {
  const record = await prisma.courseProgress.findUnique({
    where: { userId_courseId: { userId, courseId: Number(courseId) } },
  });
  return parseProgress(record);
}

// O progresso (e a nota do quiz) são sempre calculados aqui a partir do
// gabarito do curso — nunca aceitos prontos do cliente. Isso fecha a brecha
// que permitiria declarar progress:100 (e ganhar XP) direto via chamada à
// API sem de fato acertar a nota mínima de 60% no quiz.
async function upsertProgress(userId, courseId, data) {
  const numericCourseId = Number(courseId);

  const [existing, course] = await Promise.all([
    prisma.courseProgress.findUnique({ where: { userId_courseId: { userId, courseId: numericCourseId } } }),
    prisma.course.findUnique({ where: { id: numericCourseId } }),
  ]);

  const materialAccessed = data.materialAccessed !== undefined ? !!data.materialAccessed : existing?.materialAccessed || false;
  const studyCompleted = data.studyCompleted !== undefined ? !!data.studyCompleted : existing?.studyCompleted || false;
  const exercisesUnlocked = data.exercisesUnlocked !== undefined ? !!data.exercisesUnlocked : existing?.exercisesUnlocked || false;
  const quizCompleted = data.quizCompleted !== undefined ? !!data.quizCompleted : existing?.quizCompleted || false;
  const answers = data.answers !== undefined ? data.answers : (existing?.answersJson ? JSON.parse(existing.answersJson) : {});
  const openAnswer = data.openAnswer !== undefined ? data.openAnswer : existing?.openAnswer || "";

  const quiz = course ? JSON.parse(course.quizJson) : [];
  const grade = quizCompleted && quiz.length > 0
    ? gradeQuiz(quiz, answers)
    : { score: 0, totalQuestions: quiz.length, scorePercent: 0, passed: false };

  // 30% material + 70% quiz aprovado = 100%. Se não atingiu a nota mínima,
  // o quiz só contribui proporcionalmente e o total fica travado em no
  // máximo 65% (nunca libera certificado/XP sem aprovação real).
  const weightMaterial = (materialAccessed || studyCompleted) ? 30 : 0;
  const weightQuiz = quizCompleted ? (grade.passed ? 70 : Math.round((grade.scorePercent / 100) * 35)) : 0;
  const nextProgress = grade.passed
    ? Math.min(weightMaterial + weightQuiz, 100)
    : Math.min(weightMaterial + weightQuiz, 65);

  const alreadyCompletedBefore = !!(existing && existing.progress === 100 && existing.quizPassed);

  const record = await prisma.courseProgress.upsert({
    where: { userId_courseId: { userId, courseId: numericCourseId } },
    update: {
      progress: nextProgress,
      answersJson: JSON.stringify(answers),
      openAnswer,
      quizCompleted,
      quizScore: grade.score,
      quizScorePercent: grade.scorePercent,
      quizPassed: grade.passed,
      studyCompleted,
      materialAccessed,
      exercisesUnlocked,
    },
    create: {
      userId,
      courseId: numericCourseId,
      progress: nextProgress,
      answersJson: JSON.stringify(answers),
      openAnswer,
      quizCompleted,
      quizScore: grade.score,
      quizScorePercent: grade.scorePercent,
      quizPassed: grade.passed,
      studyCompleted,
      materialAccessed,
      exercisesUnlocked,
    },
  });

  let user = await prisma.user.findUnique({ where: { id: userId } });
  let leveledUp = false;
  let xpGained = 0;

  if (nextProgress === 100 && grade.passed && !alreadyCompletedBefore) {
    xpGained = XP_PER_COURSE_COMPLETION;
    const result = applyLevelUps({ xp: user.xp + xpGained, level: user.level });
    leveledUp = result.leveledUp;
    user = await prisma.user.update({
      where: { id: userId },
      data: { xp: result.xp, level: result.level },
    });
  }

  return {
    progress: parseProgress(record),
    user: toPublicUser(user),
    xpGained,
    leveledUp,
  };
}

module.exports = { getAllForUser, getOne, upsertProgress };
