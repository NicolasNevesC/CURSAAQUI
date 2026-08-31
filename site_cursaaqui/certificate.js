function generateCertificate(courseName){

  const user = JSON.parse(localStorage.getItem("loggedUser"));

  if(!user){
    showToast("Faça login primeiro.");
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