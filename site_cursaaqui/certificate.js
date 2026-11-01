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

  const modal = document.getElementById("certificateModal");
  if (modal) modal.style.display="flex";

  const userEl = document.getElementById("certUser");
  if (userEl) userEl.innerText = user.name;

  const courseEl = document.getElementById("certCourse");
  if (courseEl) courseEl.innerText = courseName;

  const dateEl = document.getElementById("certDate");
  if (dateEl) dateEl.innerText = new Date().toLocaleDateString("pt-BR");

  const hoursEl = document.getElementById("certHours");
  if (hoursEl) {
    const cObj = typeof courses !== "undefined" ? courses.find(c => c.title === courseName) : null;
    const duration = cObj && cObj.duration ? cObj.duration : "40h";
    hoursEl.innerText = `Carga Horária Oficial: ${duration}`;
  }
}

function closeCertificate(){
  const modal = document.getElementById("certificateModal");
  if (modal) modal.style.display = "none";
}

function printCertificate() {
  document.body.classList.add("printing-cert");
  window.print();
}

window.addEventListener("afterprint", () => {
  document.body.classList.remove("printing-cert");
});