const path = require("path");
const bcrypt = require("bcryptjs");
const { PrismaClient } = require("@prisma/client");

// Reaproveita o catálogo real do frontend em vez de duplicar os ~12 cursos
// aqui — courses.js já exporta via module.exports, compatível com require().
const courses = require(path.join(__dirname, "..", "..", "site_cursaaqui", "courses.js"));

const prisma = new PrismaClient();

async function seedCourses() {
  // Sem IDENTITY_INSERT (o connector sqlserver do Prisma não preserva bem
  // esse SET entre chamadas, mesmo em transação): como a tabela começa
  // vazia e courses.js já está em ordem 1..N sem lacunas, basta inserir
  // nessa ordem e deixar o SQL Server autoincrementar — o id gerado bate
  // exatamente com course.id original.
  for (const course of courses) {
    await prisma.course.upsert({
      where: { id: course.id },
      update: {},
      create: {
        title: course.title,
        category: course.category,
        area: course.area || null,
        teacherName: course.teacher,
        duration: course.duration,
        rating: course.rating || 0,
        minLevel: course.minLevel || 1,
        image: course.image || null,
        pdf: course.pdf || null,
        lessonsJson: JSON.stringify(course.lessons || []),
        quizJson: JSON.stringify(course.quiz || []),
        openQuestionJson: course.openQuestion ? JSON.stringify(course.openQuestion) : null,
      },
    });
  }
  console.log(`Cursos semeados: ${courses.length}`);
}

async function seedUsers() {
  const passwordHash = await bcrypt.hash("123456", 10);

  const teacher = await prisma.user.upsert({
    where: { email: "ricardo@cursaaqui.com" },
    update: {},
    create: {
      name: "Prof. Ricardo Silva",
      email: "ricardo@cursaaqui.com",
      password: passwordHash,
      role: "TEACHER",
      xp: 1500,
      level: 5,
    },
  });

  const student = await prisma.user.upsert({
    where: { email: "aluno@cursaaqui.com" },
    update: {},
    create: {
      name: "Matheus Narvaes",
      email: "aluno@cursaaqui.com",
      password: passwordHash,
      role: "STUDENT",
      xp: 350,
      level: 2,
    },
  });

  console.log("Usuários demo semeados: ricardo@cursaaqui.com (senha 123456), aluno@cursaaqui.com (senha 123456)");
  return { teacher, student };
}

// Recria o progresso demo que existia em auth.js (localStorage) para o
// aluno demo, para o catálogo não começar "vazio" na primeira execução.
async function seedDemoProgress(student) {
  const demoProgress = [
    { courseId: 1, progress: 85, quizCompleted: false, materialAccessed: true, quizScore: 0, quizScorePercent: 0, quizPassed: false },
    // 100% só é consistente com a regra de nota mínima (60%) se quizPassed também for true.
    { courseId: 2, progress: 100, quizCompleted: true, materialAccessed: true, quizScore: 10, quizScorePercent: 100, quizPassed: true },
  ];

  for (const entry of demoProgress) {
    await prisma.courseProgress.upsert({
      where: { userId_courseId: { userId: student.id, courseId: entry.courseId } },
      update: {},
      create: {
        userId: student.id,
        courseId: entry.courseId,
        progress: entry.progress,
        quizCompleted: entry.quizCompleted,
        quizScore: entry.quizScore,
        quizScorePercent: entry.quizScorePercent,
        quizPassed: entry.quizPassed,
        materialAccessed: entry.materialAccessed,
        studyCompleted: entry.materialAccessed,
      },
    });
  }

  // Uma submissão pendente de exemplo (as demais contas do demo antigo —
  // Ana Clara, Lucas — não existem como usuários reais, então não são
  // recriadas aqui; criar contas só para popular a fila de correção seria
  // trabalho extra sem valor real).
  await prisma.submission.upsert({
    where: { userId_courseId: { userId: student.id, courseId: 1 } },
    update: {},
    create: {
      userId: student.id,
      courseId: 1,
      answersJson: JSON.stringify({ 0: 2, 1: 1, 2: 1, 3: 2, 4: 1, 5: 1, 6: 3, 7: 2, 8: 2, 9: 0 }),
      score: 10,
      totalQuestions: 10,
      passed: true,
      status: "PENDENTE",
    },
  });

  console.log("Progresso e submissão demo semeados para aluno@cursaaqui.com");
}

async function main() {
  await seedCourses();
  const { student } = await seedUsers();
  await seedDemoProgress(student);
}

main()
  .catch((err) => {
    console.error(err);
    process.exitCode = 1;
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
