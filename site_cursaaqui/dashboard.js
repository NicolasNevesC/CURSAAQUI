// ============================================================
// dashboard.js - Painel dinâmico do Aluno (página inicial)
// Renderiza o progresso do aluno logado usando a API (Express +
// Prisma). O painel do professor foi retirado daqui: professor.html
// já tem uma implementação completa e independente (roster de
// alunos, filtros, modal de correção com questão dissertativa) —
// duplicar essa lógica aqui só divergiria com o tempo.
// ============================================================


// ─────────────────────────────────────────────────────────────
// loadDashboard()
// Ponto de entrada do dashboard da página inicial. Se o usuário
// logado for professor, direciona para o painel dedicado em vez
// de duplicar a tabela de correção aqui.
// ─────────────────────────────────────────────────────────────
async function loadDashboard() {
  const container = document.getElementById("dashboard-content");
  if (!container) return;

  const user = getCurrentUser();

  if (!user) {
    container.innerHTML = `
      <h3>Bem-vindo ao Dashboard Cursa Aqui</h3>
      <p style="margin-top: 10px;">Faça login como Aluno para acompanhar seu progresso ou como Professor para avaliar atividades.</p>
      <button class="btn-primary" style="margin-top: 15px;" onclick="openAuthModal()">Entrar Agora</button>
    `;
    return;
  }

  if (user.role === "teacher") {
    container.innerHTML = `
      <h3>Painel do Professor</h3>
      <p style="margin-top: 10px;">A correção de atividades agora tem um painel dedicado e mais completo.</p>
      <a href="professor.html" class="btn-primary" style="margin-top: 15px; display: inline-block; text-decoration: none;">Abrir Painel do Professor</a>
    `;
    return;
  }

  await renderStudentDashboard(container, user);
}


// ─────────────────────────────────────────────────────────────
// renderStudentDashboard(container, user)
// Monta e injeta o HTML do painel do aluno dentro do container.
// ─────────────────────────────────────────────────────────────
async function renderStudentDashboard(container, user) {
  const [records, coursesList, mySubmissions] = await Promise.all([
    apiFetch("/progress").catch(() => ({})),
    getAllCourses(),
    apiFetch("/submissions/mine").catch(() => []),
  ]);

  const keys = Object.keys(records);

  const userLevel = user.level || 1;
  const userXp = user.xp || 0;
  const xpNeeded = userLevel * 1000;
  const xpPercent = Math.min(100, Math.round((userXp / xpNeeded) * 100));

  let coursesHtml      = "";
  let certificatesHtml = "";

  if (keys.length === 0) {
    coursesHtml = `
      <div style="text-align: center; padding: 30px; background: rgba(255,255,255,0.03); border-radius: 14px; border: 1px dashed rgba(255,255,255,0.15);">
        <p style="font-size: 1rem; opacity: 0.85; margin-bottom: 12px;">Você ainda não iniciou nenhum curso.</p>
        <a href="cursos.html" class="btn-primary" style="text-decoration: none; display: inline-block; padding: 8px 18px;">
          🚀 Explorar Catálogo de Cursos
        </a>
      </div>
    `;
    certificatesHtml = `<p style="font-style: italic; opacity: 0.7;">Nenhum certificado disponível no momento.</p>`;
  } else {
    keys.forEach(id => {
      const record = records[id];
      const foundCourse = coursesList.find(c => c.id === parseInt(id));
      const title = foundCourse ? foundCourse.title : "Curso";
      const pdfPath = (foundCourse && foundCourse.pdf) ? foundCourse.pdf : "material.pdf";
      const safeTitle = title.replace(/'/g, "\\'");
      const isDone = record.progress === 100 && record.quizPassed === true;

      coursesHtml += `
        <div class="dash-student-course-item" data-status="${isDone ? 'completed' : 'in_progress'}" style="background: rgba(255,255,255,0.04); padding: 16px; border-radius: 12px; margin-bottom: 14px; border: 1px solid rgba(255,255,255,0.08); transition: transform .2s, border-color .2s;">
          <div style="display: flex; justify-content: space-between; align-items: flex-start; flex-wrap: wrap; gap: 8px; margin-bottom: 8px;">
            <div>
              <strong style="font-size: 1rem; color: var(--text);">${title}</strong>
              <div style="font-size: 0.78rem; opacity: 0.7; margin-top: 2px;">Carga Horária Estimada: 40h • Apostila Digital</div>
            </div>
            <span class="category-tag" style="background: ${isDone ? 'rgba(16,185,129,0.2)' : 'rgba(59,130,246,0.2)'}; color: ${isDone ? '#10B981' : '#60A5FA'}; font-weight: 700;">
              ${record.progress}% Concluído
            </span>
          </div>

          <div style="width: 100%; height: 7px; background: rgba(255,255,255,0.08); border-radius: 10px; overflow: hidden; margin-bottom: 12px;">
            <div style="width: ${record.progress}%; height: 100%; background: linear-gradient(90deg, #3B82F6, #10B981); border-radius: 10px;"></div>
          </div>

          <div style="display: flex; gap: 8px; flex-wrap: wrap; align-items: center;">
            <button class="btn-primary" style="padding: 7px 14px; font-size: 0.82rem;" onclick="watchCourse('${safeTitle}')">
              🎓 Ir para Aula
            </button>
            <a href="${pdfPath}" target="_blank" class="btn-secondary" style="padding: 7px 14px; font-size: 0.82rem; text-decoration: none; display: inline-flex; align-items: center; gap: 5px;">
              📖 Material (PDF)
            </a>
            ${isDone ? `
              <button class="btn-primary" style="padding: 7px 14px; font-size: 0.82rem; background: #10B981;" onclick="gerarCertificadoDoPainel('${safeTitle}')">
                🏆 Emitir Certificado
              </button>
            ` : ''}
          </div>
        </div>
      `;

      if (isDone) {
        certificatesHtml += `
          <div style="background: rgba(16,185,129,0.06); border: 1px solid rgba(16,185,129,0.2); padding: 14px 18px; border-radius: 10px; margin-bottom: 10px; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 10px;">
            <div>
              <span style="font-weight: 700; font-size: 0.95rem; color: #10B981;">🏆 ${title}</span>
              <div style="font-size: 0.78rem; opacity: 0.8; margin-top: 2px;">Concluído com aproveitamento máximo</div>
            </div>
            <button class="btn-primary" style="padding: 6px 14px; font-size: 0.85rem;" onclick="gerarCertificadoDoPainel('${safeTitle}')">
              Visualizar Certificado
            </button>
          </div>
        `;
      }
    });

    if (certificatesHtml === "") {
      certificatesHtml = `<p style="font-style: italic; opacity: 0.7;">Complete 100% de um curso para liberar seu certificado.</p>`;
    }
  }

  // ─── Pareceres dos professores ──────────────────────────────
  let feedbackHtml = "";
  if (mySubmissions.length > 0) {
    mySubmissions.forEach(sub => {
      const isDone = sub.status === "CORRIGIDO";
      const courseTitle = sub.course ? sub.course.title : "Curso";
      feedbackHtml += `
        <div style="background: ${isDone ? 'rgba(16, 185, 129, 0.08)' : 'rgba(245, 158, 11, 0.08)'}; border-left: 4px solid ${isDone ? '#10B981' : '#F59E0B'}; padding: 14px 18px; border-radius: 8px; margin-bottom: 12px;">
          <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 6px;">
            <div style="font-weight: 700; color: ${isDone ? '#10B981' : '#F59E0B'};">📘 ${courseTitle}</div>
            <span style="font-size: 0.75rem; font-weight: 700; padding: 2px 8px; border-radius: 12px; background: rgba(255,255,255,0.1);">
              ${isDone ? '✅ Corrigido' : '⏳ Pendente'}
            </span>
          </div>
          <div style="font-size: 0.85rem; margin-top: 6px; opacity: 0.9;">
            <strong>Nota:</strong> ${sub.score} / ${sub.totalQuestions} acertos (${Math.round((sub.score / sub.totalQuestions) * 100)}%)
          </div>
          <div style="font-size: 0.85rem; margin-top: 6px; font-style: italic; opacity: 0.95;">
            " ${sub.feedback || (isDone ? 'Atividade avaliada e aprovada pelo professor.' : 'Em processo de correção pela coordenação pedagógica.')} "
          </div>
        </div>
      `;
    });
  }

  if (feedbackHtml === "") {
    feedbackHtml = `<p style="font-style: italic; opacity: 0.7;">Você ainda não possui pareceres emitidos por professores.</p>`;
  }

  container.innerHTML = `
    <div style="text-align: left;">
      <div style="background: linear-gradient(135deg, #1E3A8A 0%, #3B82F6 60%, #8B5CF6 100%); border-radius: 16px; padding: 22px 26px; margin-bottom: 24px; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 16px; box-shadow: 0 8px 24px rgba(59,130,246,0.25);">
        <div>
          <span style="background: rgba(255,255,255,0.2); color: #fff; font-size: 0.72rem; font-weight: 700; padding: 3px 10px; border-radius: 14px; text-transform: uppercase; letter-spacing: .05em;">NOVA ÁREA DO ALUNO</span>
          <h3 style="color: #fff; font-size: 1.35rem; font-weight: 900; margin: 6px 0 4px 0;">Painel 100% Interativo do Aluno</h3>
          <p style="color: rgba(255,255,255,0.85); font-size: 0.88rem; max-width: 540px;">
            Acesse seu cronograma, metas de estudo, bloco de anotações pessoal, simulador de questões com ganho de XP e galeria de diplomas.
          </p>
        </div>
        <a href="aluno.html" class="btn-primary" style="background: #fff; color: #1E3A8A; font-weight: 800; padding: 12px 22px; font-size: 0.92rem; border-radius: 10px; text-decoration: none; box-shadow: 0 4px 14px rgba(0,0,0,0.15); display: inline-flex; align-items: center; gap: 8px;">
          🚀 Abrir Painel Completo do Aluno
        </a>
      </div>

      <div style="background: rgba(255,255,255,0.04); border: 1px solid rgba(255,255,255,0.08); border-radius: 14px; padding: 18px 22px; margin-bottom: 24px; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 14px;">
        <div>
          <h3 style="margin-bottom: 4px;">Olá, ${user.name}! 👋</h3>
          <p style="opacity: 0.8; font-size: 0.88rem;">Nível Atual: <strong>Nível ${userLevel}</strong> • XP Total: <strong>${userXp} / ${xpNeeded} XP</strong></p>
          <div style="width: 240px; height: 6px; background: rgba(255,255,255,0.1); border-radius: 10px; overflow: hidden; margin-top: 8px;">
            <div style="width: ${xpPercent}%; height: 100%; background: linear-gradient(90deg, #3B82F6, #10B981);"></div>
          </div>
        </div>
        <div style="display: flex; gap: 10px; flex-wrap: wrap;">
          <a href="aluno.html" class="btn-primary" style="padding: 8px 16px; font-size: 0.85rem; text-decoration: none;">
            🎯 Gerenciar Metas &amp; Notas
          </a>
          <a href="cursos.html" class="btn-secondary" style="padding: 8px 16px; font-size: 0.85rem; text-decoration: none;">
            + Explorar Matérias
          </a>
        </div>
      </div>

      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 14px; flex-wrap: wrap; gap: 10px;">
        <h4 style="font-size: 1.05rem; margin: 0;">📈 Teu Progresso nos Cursos &amp; Materiais</h4>
        <div style="display: flex; gap: 6px;">
          <button class="btn-secondary" style="padding: 4px 10px; font-size: 0.78rem;" onclick="filterInlineDashboardCourses('all')">Todos</button>
          <button class="btn-secondary" style="padding: 4px 10px; font-size: 0.78rem;" onclick="filterInlineDashboardCourses('in_progress')">Em Andamento</button>
          <button class="btn-secondary" style="padding: 4px 10px; font-size: 0.78rem;" onclick="filterInlineDashboardCourses('completed')">Concluídos</button>
        </div>
      </div>

      <div id="inlineDashboardCoursesList" style="margin-bottom: 25px;">${coursesHtml}</div>

      <h4 style="margin-bottom: 12px; font-size: 1.05rem;">📝 Pareceres dos Professores</h4>
      <div style="margin-bottom: 25px;">${feedbackHtml}</div>

      <h4 style="margin-bottom: 12px; font-size: 1.05rem;">📜 Teus Certificados Conquistados</h4>
      <div>${certificatesHtml}</div>
    </div>
  `;
}

// Filtro rápido dos cards no dashboard inline
function filterInlineDashboardCourses(status) {
  const items = document.querySelectorAll(".dash-student-course-item");
  items.forEach(item => {
    if (status === 'all' || item.getAttribute('data-status') === status) {
      item.style.display = "block";
    } else {
      item.style.display = "none";
    }
  });
}


// ─────────────────────────────────────────────────────────────
// gerarCertificadoDoPainel(courseTitle)
// Abre o modal de certificado para um curso concluído.
// ─────────────────────────────────────────────────────────────
function gerarCertificadoDoPainel(courseTitle) {
  if (typeof generateCertificate === "function") {
    generateCertificate(courseTitle);
  } else {
    const user = getCurrentUser();
    if(document.getElementById("certUser"))   document.getElementById("certUser").innerText   = user.name;
    if(document.getElementById("certCourse")) document.getElementById("certCourse").innerText = courseTitle;
    if(document.getElementById("certDate"))   document.getElementById("certDate").innerText   = new Date().toLocaleDateString('pt-BR');
    if(document.getElementById("certificateModal")) document.getElementById("certificateModal").style.display = "flex";
  }
}


// ─────────────────────────────────────────────────────────────
// Inicialização automática
// ─────────────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", loadDashboard);
