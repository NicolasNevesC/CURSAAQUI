function generateCertificate(courseName){

  const user = JSON.parse(localStorage.getItem("loggedUser"));

  if(!user){
    showToast("Faça login primeiro.");
    return;
  }

  // Validação de nota mínima (60%) e progresso de 100%
  const progressKey = `user_courses_progress_${user.email}`;
  const records = JSON.parse(localStorage.getItem(progressKey)) || {};
  const rec = Object.values(records).find(r => r.title === courseName);
  const selectedCourse = JSON.parse(localStorage.getItem("selectedCourse"));
  const targetRec = rec || (selectedCourse && selectedCourse.title === courseName ? selectedCourse : null);

  if (targetRec) {
    if (targetRec.progress < 100 || targetRec.quizPassed === false) {
      const nota = targetRec.quizScorePercent !== undefined ? targetRec.quizScorePercent : 0;
      showToast(`🔒 Certificado bloqueado: aproveitamento de ${nota}%. A nota mínima exigida para aprovação é 60%.`);
      return;
    }
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