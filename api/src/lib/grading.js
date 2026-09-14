// Nota mínima de aprovação para liberar certificado/XP. Compartilhado entre
// progressService e submissionService para que os dois nunca divirjam sobre
// o que conta como "aprovado".
const PASSING_PERCENT = 60;

// Corrige o quiz a partir do gabarito do curso (nunca do que o cliente diz
// que acertou) — quem chama passa `quiz` (já desserializado) e `answers`
// ({questionIndex: selectedOptionIndex}).
function gradeQuiz(quiz, answers) {
  const totalQuestions = quiz.length;
  let score = 0;
  quiz.forEach((q, idx) => {
    if (answers && answers[idx] === q.correct) score++;
  });
  const scorePercent = totalQuestions > 0 ? Math.round((score / totalQuestions) * 100) : 0;
  const passed = scorePercent >= PASSING_PERCENT;
  return { score, totalQuestions, scorePercent, passed };
}

module.exports = { gradeQuiz, PASSING_PERCENT };
