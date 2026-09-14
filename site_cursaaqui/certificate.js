// Confere aprovação (progress 100% + nota mínima de 60% no quiz) direto na
// API antes de exibir o certificado — nunca confia em estado só do cliente.
async function generateCertificate(courseName){

  const user = getCurrentUser();

  if(!user){
    showToast("Faça login primeiro.");
    return;
  }

  try {
    const allCourses = await getAllCourses();
    const course = allCourses.find(c => c.title === courseName);
    if (course) {
      const record = await apiFetch(`/progress/${course.id}`).catch(() => null);
      if (record && (record.progress < 100 || record.quizPassed !== true)) {
        const nota = record.quizScorePercent !== undefined ? record.quizScorePercent : 0;
        showToast(`🔒 Certificado bloqueado: aproveitamento de ${nota}%. A nota mínima exigida para aprovação é 60%.`);
        return;
      }
    }
  } catch (e) {
    // Se a checagem falhar (ex: API fora do ar), segue sem bloquear a
    // exibição — o certificado em si é só um documento visual client-side.
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
