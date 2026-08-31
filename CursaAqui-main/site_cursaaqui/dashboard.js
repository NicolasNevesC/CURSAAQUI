// dashboard.js - Atualização dinâmica do Painel do Aluno

function loadDashboard() {
  const container = document.getElementById("dashboard-content");
  if (!container) return;

  const user = JSON.parse(localStorage.getItem("loggedUser"));

  if (!user) {
    container.innerHTML = `
      <h3>Bem-vindo ao teu Dashboard</h3>
      <p style="margin-top: 10px;">Faz login para acompanhar o teu progresso pessoal, aulas concluídas e certificados emitidos.</p>
      <button class="btn-primary" style="margin-top: 15px;" onclick="openAuthModal()">Entrar Agora</button>
    `;
    return;
  }

  const progressKey = `user_courses_progress_${user.email}`;
  const records = JSON.parse(localStorage.getItem(progressKey)) || {};
  const keys = Object.keys(records);

  let coursesHtml = "";
  let certificatesHtml = "";

  if (keys.length === 0) {
    coursesHtml = `<p style="font-style: italic; opacity: 0.7;">Você ainda não iniciou nenhum curso.</p>`;
    certificatesHtml = `<p style="font-style: italic; opacity: 0.7;">Nenhum certificado disponível.</p>`;
  } else {
    keys.forEach(id => {
      const record = records[id];
      
      coursesHtml += `
        <div style="background: rgba(255,255,255,0.05); padding: 12px; border-radius: 8px; margin-bottom: 10px; display: flex; justify-content: space-between; align-items: center;">
          <span style="font-weight: 500;">${record.title}</span>
          <span class="category-tag">${record.progress}% Concluído</span>
        </div>
      `;

      if (record.progress === 100) {
        certificatesHtml += `
          <div style="background: rgba(255,255,255,0.05); padding: 12px; border-radius: 8px; margin-bottom: 10px; display: flex; justify-content: space-between; align-items: center;">
            <span style="font-weight: 600;">🏆 ${record.title}</span>
            <button class="btn-primary" style="padding: 5px 12px; font-size: 0.85rem;" onclick="gerarCertificadoDoPainel('${record.title}')">Visualizar</button>
          </div>
        `;
      }
    });

    if (certificatesHtml === "") {
      certificatesHtml = `<p style="font-style: italic; opacity: 0.7;">Complete 100% de um curso para emitir seu certificado.</p>`;
    }
  }

  container.innerHTML = `
    <div style="text-align: left;">
      <h3 style="border-bottom: 2px solid rgba(255,255,255,0.1); padding-bottom: 10px; margin-bottom: 15px;">Olá, ${user.name}! ${user.role === 'teacher' ? '(Painel do Professor)' : ''}</h3>
      <p style="margin-bottom: 20px;">Nível Atual: <strong>${user.level || 1}</strong> | XP Total: <strong>${user.xp || 0} XP</strong></p>
      
      <h4 style="margin-bottom: 10px;">📈 Teu Progresso nos Cursos</h4>
      <div style="margin-bottom: 25px;">${coursesHtml}</div>

      <h4 style="margin-bottom: 10px;">📜 Teus Certificados Conquistados</h4>
      <div>${certificatesHtml}</div>
    </div>
  `;
}

function gerarCertificadoDoPainel(courseTitle) {
  if (typeof generateCertificate === "function") {
    generateCertificate(courseTitle);
  } else {
    const user = JSON.parse(localStorage.getItem("loggedUser"));
    if(document.getElementById("certUser")) document.getElementById("certUser").innerText = user.name;
    if(document.getElementById("certCourse")) document.getElementById("certCourse").innerText = courseTitle;
    if(document.getElementById("certDate")) document.getElementById("certDate").innerText = new Date().toLocaleDateString('pt-BR');
    if(document.getElementById("certificateModal")) document.getElementById("certificateModal").style.display = "flex";
  }
}

document.addEventListener("DOMContentLoaded", loadDashboard);