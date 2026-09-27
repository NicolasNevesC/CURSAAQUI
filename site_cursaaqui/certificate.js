// ─────────────────────────────────────────────────────────────
// checkCertificateEligibility(user, courseName)
// Função pura: decide se o usuário logado pode emitir o
// certificado do curso informado. Nega por padrão: só libera
// quando existe registro de progresso DO PRÓPRIO USUÁRIO para
// o curso, com progress >= 100 e quizPassed === true.
// Retorna { ok: true, record } ou { ok: false, reason, message }.
// ─────────────────────────────────────────────────────────────
function checkCertificateEligibility(user, courseName){
  const progressKey = `user_courses_progress_${user.email}`;
  const records = JSON.parse(localStorage.getItem(progressKey)) || {};
  const rec = Object.values(records).find(r => r.title === courseName);

  if (!rec) {
    return { ok: false, reason: "not_started", message: "🔒 Você ainda não iniciou este curso." };
  }

  if (rec.progress < 100) {
    return { ok: false, reason: "incomplete", message: `🔒 Conclua 100% do curso para liberar o certificado (atual: ${rec.progress}%).` };
  }

  if (rec.quizPassed !== true) {
    const nota = rec.quizScorePercent !== undefined ? rec.quizScorePercent : 0;
    return { ok: false, reason: "quiz_failed", message: `🔒 Certificado bloqueado: aproveitamento de ${nota}%. A nota mínima exigida é 60%.` };
  }

  return { ok: true, record: rec };
}

function generateCertificate(courseName){

  const user = Storage.getLoggedUser();

  if(!user){
    showToast("Faça login primeiro.");
    return;
  }

  const eligibility = checkCertificateEligibility(user, courseName);
  if (!eligibility.ok) {
    showToast(eligibility.message);
    return;
  }

  document.getElementById("certificateModal").style.display="flex";

  document.getElementById("certUser").innerText = user.name;

  document.getElementById("certCourse").innerText = courseName;

  document.getElementById("certDate").innerText =
    new Date().toLocaleDateString("pt-BR");
}

function closeCertificate(){
  document.getElementById("certificateModal").style.display = "none";
}