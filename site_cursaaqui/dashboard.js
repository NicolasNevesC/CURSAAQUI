// ============================================================
// dashboard.js - Painel dinâmico do Aluno e do Professor
// Renderiza o dashboard correto dependendo do usuário logado:
//   - Aluno  → progresso dos cursos, pareceres e certificados
//   - Professor → tabela de atividades enviadas para correção
// ============================================================


// ID da submissão que está sendo corrigida no momento (global)
// Guardado fora das funções para ser acessível por salvarCorrecaoProfessor()
let activeCorrectionId = null;


// ─────────────────────────────────────────────────────────────
// loadDashboard()
// Ponto de entrada do dashboard. Lê o usuário logado e decide
// qual painel renderizar. Se não houver ninguém logado, exibe
// um convite para entrar na plataforma.
// ─────────────────────────────────────────────────────────────
function loadDashboard() {
  // Encontra o elemento container onde o dashboard será injetado
  const container = document.getElementById("dashboard-content");
  if (!container) return; // sai se o elemento não existir na página

  // Lê o usuário atualmente logado do localStorage
  const user = JSON.parse(localStorage.getItem("loggedUser"));

  // Ninguém logado: exibe mensagem de boas-vindas com botão de login
  if (!user) {
    container.innerHTML = `
      <h3>Bem-vindo ao Dashboard Cursa Aqui</h3>
      <p style="margin-top: 10px;">Faça login como Aluno para acompanhar seu progresso ou como Professor para avaliar atividades.</p>
      <button class="btn-primary" style="margin-top: 15px;" onclick="openAuthModal()">Entrar Agora</button>
    `;
    return;
  }

  // Redireciona para o painel correto conforme o perfil do usuário
  if (user.role === "teacher") {
    renderTeacherDashboard(container, user);
  } else {
    renderStudentDashboard(container, user);
  }
}


// ─────────────────────────────────────────────────────────────
// renderStudentDashboard(container, user)
// Monta e injeta o HTML do painel do aluno dentro do container.
// Exibe: cursos em andamento, pareceres de professores e
// certificados conquistados (100% de progresso).
// ─────────────────────────────────────────────────────────────
function renderStudentDashboard(container, user) {
  // Chave única por aluno para recuperar o progresso salvo
  const progressKey = `user_courses_progress_${user.email}`;
  const records = JSON.parse(localStorage.getItem(progressKey)) || {};
  const keys = Object.keys(records); // IDs dos cursos que o aluno já iniciou
  const coursesList = window.courses || (typeof courses !== "undefined" ? courses : []);

  const userLevel = user.level || 1;
  const userXp = user.xp || 0;
  const xpNeeded = userLevel * 1000;
  const xpPercent = Math.min(100, Math.round((userXp / xpNeeded) * 100));

  let coursesHtml      = ""; // HTML dos cursos em andamento
  let certificatesHtml = ""; // HTML dos certificados disponíveis

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
      const foundCourse = coursesList.find(c => c.id === parseInt(id) || c.title === record.title);
      const pdfPath = (foundCourse && foundCourse.pdf) ? foundCourse.pdf : (record.pdf || "material.pdf");
      const safeTitle = record.title.replace(/'/g, "\\'");
      const isDone = record.progress === 100;

      // Card moderno do curso com barra de progresso visual
      coursesHtml += `
        <div class="dash-student-course-item" data-status="${isDone ? 'completed' : 'in_progress'}" style="background: rgba(255,255,255,0.04); padding: 16px; border-radius: 12px; margin-bottom: 14px; border: 1px solid rgba(255,255,255,0.08); transition: transform .2s, border-color .2s;">
          <div style="display: flex; justify-content: space-between; align-items: flex-start; flex-wrap: wrap; gap: 8px; margin-bottom: 8px;">
            <div>
              <strong style="font-size: 1rem; color: var(--text);">${record.title}</strong>
              <div style="font-size: 0.78rem; opacity: 0.7; margin-top: 2px;">Carga Horária Estimada: 40h • Apostila Digital</div>
            </div>
            <span class="category-tag" style="background: ${isDone ? 'rgba(16,185,129,0.2)' : 'rgba(59,130,246,0.2)'}; color: ${isDone ? '#10B981' : '#60A5FA'}; font-weight: 700;">
              ${record.progress}% Concluído
            </span>
          </div>

          <!-- Barra de progresso visual -->
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
              <span style="font-weight: 700; font-size: 0.95rem; color: #10B981;">🏆 ${record.title}</span>
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
  const submissions  = JSON.parse(localStorage.getItem("teacher_activity_submissions")) || [];
  const mySubmissions = submissions.filter(s => s.userEmail === user.email);

  let feedbackHtml = "";
  if (mySubmissions.length > 0) {
    mySubmissions.forEach(sub => {
      const isDone = sub.status === "Corrigido";
      feedbackHtml += `
        <div style="background: ${isDone ? 'rgba(16, 185, 129, 0.08)' : 'rgba(245, 158, 11, 0.08)'}; border-left: 4px solid ${isDone ? '#10B981' : '#F59E0B'}; padding: 14px 18px; border-radius: 8px; margin-bottom: 12px;">
          <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 6px;">
            <div style="font-weight: 700; color: ${isDone ? '#10B981' : '#F59E0B'};">📘 ${sub.courseTitle}</div>
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

  // ─── Injeta o HTML final do painel do aluno no container ────
  container.innerHTML = `
    <div style="text-align: left;">
      <!-- Banner de Destaque para o Novo Dashboard Dedicado -->
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

      <!-- Resumo do Perfil -->
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

      <!-- Filtros Rápidos de Cursos -->
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
// renderTeacherDashboard(container, user)
// Monta e injeta o HTML do painel do professor.
// Exibe: cards de resumo (total, pendentes, corrigidos) e uma
// tabela com todas as atividades enviadas pelos alunos.
// Também injeta o modal de correção de atividade no DOM.
// ─────────────────────────────────────────────────────────────
function renderTeacherDashboard(container, user) {
  // Carrega todas as submissões de alunos
  const submissions    = JSON.parse(localStorage.getItem("teacher_activity_submissions")) || [];
  const totalSubmissions = submissions.length;
  const pendingCount   = submissions.filter(s => s.status === "Pendente").length;
  const correctedCount = submissions.filter(s => s.status === "Corrigido").length;

  // ─── Monta as linhas da tabela de submissões ─────────────────
  let tableRows = "";
  if (submissions.length === 0) {
    // Nenhuma atividade enviada ainda: exibe linha vazia
    tableRows = `<tr><td colspan="6" style="padding: 20px; text-align: center; opacity: 0.7;">Nenhuma atividade enviada por alunos até o momento.</td></tr>`;
  } else {
    submissions.forEach(sub => {
      const isPending = sub.status === "Pendente";

      // Badge colorido de status: laranja = pendente, verde = corrigido
      const statusBadge = isPending 
        ? `<span style="background: rgba(245, 158, 11, 0.2); color: #F59E0B; padding: 4px 10px; border-radius: 20px; font-weight: 600; font-size: 0.8rem;">⏳ Pendente</span>`
        : `<span style="background: rgba(16, 185, 129, 0.2); color: #10B981; padding: 4px 10px; border-radius: 20px; font-weight: 600; font-size: 0.8rem;">✅ Corrigido</span>`;

      // Cada linha da tabela com dados do aluno, curso, nota, status e botão de correção
      tableRows += `
        <tr style="border-bottom: 1px solid rgba(255,255,255,0.08); text-align: left;">
          <td style="padding: 12px;"><strong>${sub.userName}</strong><br><small style="opacity: 0.7;">${sub.userEmail}</small></td>
          <td style="padding: 12px;">${sub.courseTitle}</td>
          <td style="padding: 12px; font-weight: 600;">${sub.score} / ${sub.totalQuestions} pts</td>
          <td style="padding: 12px;">${statusBadge}</td>
          <td style="padding: 12px; font-size: 0.85rem; max-width: 220px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
            ${sub.feedback ? sub.feedback : '<span style="opacity:0.5;">Sem parecer</span>'}
          </td>
          <td style="padding: 12px; text-align: right;">
            <button class="btn-primary" style="padding: 6px 12px; font-size: 0.85rem;" onclick="abrirModalCorrecao(${sub.id})">
              📝 Corrigir &amp; Dar Feedback
            </button>
          </td>
        </tr>
      `;
    });
  }

  // ─── Injeta o HTML completo do painel do professor ───────────
  container.innerHTML = `
    <div style="text-align: left;">
      <div style="display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid rgba(255,255,255,0.1); padding-bottom: 15px; margin-bottom: 20px;">
        <div>
          <h2>👨‍🏫 Painel de Correção de Atividades do Professor</h2>
          <p style="opacity: 0.8; font-size: 0.95rem; margin-top: 4px;">Bem-vindo(a), <strong>${user.name}</strong>! Avalie os questionários e envie pareceres aos seus alunos.</p>
        </div>
      </div>

      <!-- Cards de resumo numérico -->
      <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 15px; margin-bottom: 30px;">
        <div style="background: rgba(59, 130, 246, 0.1); border: 1px solid rgba(59, 130, 246, 0.2); padding: 18px; border-radius: 12px; text-align: center;">
          <div style="font-size: 1.8rem; font-weight: 800; color: #3B82F6;">${totalSubmissions}</div>
          <div style="font-size: 0.85rem; opacity: 0.8; margin-top: 4px;">Total de Atividades</div>
        </div>
        <div style="background: rgba(245, 158, 11, 0.1); border: 1px solid rgba(245, 158, 11, 0.2); padding: 18px; border-radius: 12px; text-align: center;">
          <div style="font-size: 1.8rem; font-weight: 800; color: #F59E0B;">${pendingCount}</div>
          <div style="font-size: 0.85rem; opacity: 0.8; margin-top: 4px;">Pendentes de Correção</div>
        </div>
        <div style="background: rgba(16, 185, 129, 0.1); border: 1px solid rgba(16, 185, 129, 0.2); padding: 18px; border-radius: 12px; text-align: center;">
          <div style="font-size: 1.8rem; font-weight: 800; color: #10B981;">${correctedCount}</div>
          <div style="font-size: 0.85rem; opacity: 0.8; margin-top: 4px;">Atividades Corrigidas</div>
        </div>
      </div>

      <h3 style="margin-bottom: 15px;">📋 Questionários Enviados para Correção</h3>
      <div style="overflow-x: auto; background: rgba(0,0,0,0.03); border-radius: 12px; border: 1px solid rgba(0,0,0,0.08); padding: 10px;">
        <table style="width: 100%; border-collapse: collapse;">
          <thead>
            <tr style="border-bottom: 2px solid rgba(0,0,0,0.1); text-align: left; font-size: 0.9rem; opacity: 0.8;">
              <th style="padding: 12px;">Aluno</th>
              <th style="padding: 12px;">Curso</th>
              <th style="padding: 12px;">Nota</th>
              <th style="padding: 12px;">Status</th>
              <th style="padding: 12px;">Parecer</th>
              <th style="padding: 12px; text-align: right;">Ação</th>
            </tr>
          </thead>
          <tbody>
            ${tableRows}
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal de Correção: injetado aqui e controlado por abrirModalCorrecao() -->
    <div id="correctionModal" class="modal" style="display: none;">
      <div class="modal-content" style="max-width: 650px; text-align: left; max-height: 90vh; overflow-y: auto;">
        <span class="close-cert" style="position:absolute; top:15px; right:20px; cursor:pointer;" onclick="fecharModalCorrecao()">&times;</span>
        <h3 id="modalCorrectionTitle">📝 Correção de Atividade</h3>
        <p id="modalCorrectionStudent" style="opacity: 0.85; margin-bottom: 15px; font-size: 0.95rem;"></p>

        <!-- Questões do questionário são injetadas aqui por abrirModalCorrecao() -->
        <div id="modalCorrectionQuestions" style="margin-bottom: 20px;"></div>

        <div style="background: rgba(59, 130, 246, 0.05); border: 1px solid rgba(59, 130, 246, 0.2); padding: 15px; border-radius: 10px; margin-bottom: 15px;">
          <label style="font-weight: 600; display: block; margin-bottom: 6px;">💬 Parecer Técnico do Professor / Feedback:</label>
          <textarea id="modalFeedbackText" rows="3" placeholder="Escreva o parecer orientativo para o aluno..." style="width: 100%; padding: 10px; border-radius: 8px; border: 1px solid rgba(0,0,0,0.15); background: var(--card); color: inherit; font-family: inherit;"></textarea>
        </div>

        <div style="display: flex; gap: 10px; justify-content: flex-end;">
          <button class="btn-secondary" onclick="fecharModalCorrecao()">Cancelar</button>
          <button class="btn-primary" onclick="salvarCorrecaoProfessor()">✅ Salvar Correção &amp; Enviar Nota</button>
        </div>
      </div>
    </div>
  `;
}


// ─────────────────────────────────────────────────────────────
// abrirModalCorrecao(submissionId)
// Abre o modal de correção para uma submissão específica.
// Preenche o modal com: título do curso, dados do aluno,
// e cada questão do quiz com destaque visual nas respostas.
// ─────────────────────────────────────────────────────────────
function abrirModalCorrecao(submissionId) {
  // Salva o ID globalmente para uso posterior em salvarCorrecaoProfessor()
  activeCorrectionId = submissionId;

  const submissions = JSON.parse(localStorage.getItem("teacher_activity_submissions")) || [];
  const sub = submissions.find(s => s.id === submissionId);
  if (!sub) return; // submissão não encontrada

  const modal = document.getElementById("correctionModal");
  if (!modal) return; // modal não existe no DOM ainda

  // Preenche o cabeçalho do modal com info do curso e do aluno
  document.getElementById("modalCorrectionTitle").innerText = `📝 Corrigir: ${sub.courseTitle}`;
  document.getElementById("modalCorrectionStudent").innerText = `Aluno: ${sub.userName} (${sub.userEmail}) | Enviado em: ${sub.submittedAt}`;
  document.getElementById("modalFeedbackText").value = sub.feedback || "";

  const questionsContainer = document.getElementById("modalCorrectionQuestions");
  questionsContainer.innerHTML = ""; // limpa questões anteriores

  if (sub.quiz && sub.quiz.length > 0) {
    // Renderiza cada questão com coloração por resultado
    sub.quiz.forEach((q, idx) => {
      const selectedOpt = sub.answers ? sub.answers[idx] : undefined;
      const isCorrect   = selectedOpt === q.correct;

      // Ícone e cor do status dependem se o aluno acertou
      const statusIcon  = isCorrect ? '✅ Resposta Correta' : '❌ Resposta Incorreta';
      const statusColor = isCorrect ? '#10B981' : '#EF4444';

      let optionsList = "";
      q.options.forEach((opt, optIdx) => {
        let isUserChoice    = selectedOpt === optIdx; // esta foi a opção escolhida pelo aluno
        let isCorrectChoice = q.correct === optIdx;   // esta é a resposta certa

        // Estilo base de cada opção
        let style = "padding: 8px 12px; border-radius: 6px; margin-bottom: 6px; font-size: 0.85rem; border: 1px solid rgba(0,0,0,0.1);";

        // Verde: aluno marcou a opção certa
        if (isUserChoice && isCorrectChoice) {
          style += " background: rgba(16, 185, 129, 0.15); border-color: #10B981; font-weight: 600;";
        }
        // Vermelho: aluno marcou mas errou
        else if (isUserChoice && !isCorrectChoice) {
          style += " background: rgba(239, 68, 68, 0.15); border-color: #EF4444; font-weight: 600;";
        }
        // Azul: era a resposta certa, mas o aluno não marcou
        else if (isCorrectChoice) {
          style += " background: rgba(59, 130, 246, 0.1); border-color: #3B82F6;";
        }

        // Adiciona indicador visual se o aluno escolheu esta opção
        optionsList += `<div style="${style}">${opt} ${isUserChoice ? '👈 <em>(Marcado pelo Aluno)</em>' : ''}</div>`;
      });

      // Bloco de uma questão com resultado e lista de opções coloridas
      questionsContainer.innerHTML += `
        <div style="background: rgba(255,255,255,0.03); border: 1px solid rgba(0,0,0,0.08); padding: 15px; border-radius: 10px; margin-bottom: 12px;">
          <div style="display: flex; justify-content: space-between; font-weight: 600; margin-bottom: 8px;">
            <span>Questão ${idx + 1}: ${q.question}</span>
            <span style="color: ${statusColor}; font-size: 0.85rem;">${statusIcon}</span>
          </div>
          ${optionsList}
        </div>
      `;
    });
  } else {
    // Quiz sem questões detalhadas: exibe apenas a pontuação total
    questionsContainer.innerHTML = `<p style="opacity: 0.7;">Questões enviadas registradas com sucesso (${sub.score}/${sub.totalQuestions} acertos).</p>`;
  }

  modal.style.display = "flex"; // abre o modal centralizado
}


// ─────────────────────────────────────────────────────────────
// fecharModalCorrecao()
// Esconde o modal de correção sem salvar nada.
// ─────────────────────────────────────────────────────────────
function fecharModalCorrecao() {
  const modal = document.getElementById("correctionModal");
  if (modal) modal.style.display = "none";
}


// ─────────────────────────────────────────────────────────────
// salvarCorrecaoProfessor()
// Lê o feedback digitado pelo professor, atualiza o status da
// submissão para "Corrigido" e salva tudo no localStorage.
// Após salvar, fecha o modal e recarrega o dashboard.
// ─────────────────────────────────────────────────────────────
function salvarCorrecaoProfessor() {
  // Garante que existe uma submissão ativa para corrigir
  if (!activeCorrectionId) return;

  const submissions = JSON.parse(localStorage.getItem("teacher_activity_submissions")) || [];

  // Encontra o índice da submissão no array para poder editá-la
  const subIdx = submissions.findIndex(s => s.id === activeCorrectionId);
  if (subIdx === -1) return; // submissão não encontrada

  const feedback = document.getElementById("modalFeedbackText").value.trim();

  // Atualiza os campos da submissão corrigida
  submissions[subIdx].status   = "Corrigido";
  submissions[subIdx].feedback = feedback || "Atividade avaliada e aprovada pelo professor.";
  submissions[subIdx].gradedAt = new Date().toLocaleDateString('pt-BR'); // data de correção

  // Persiste as alterações no localStorage
  localStorage.setItem("teacher_activity_submissions", JSON.stringify(submissions));

  if (typeof showToast === "function") {
    showToast("Correção e parecer salvos com sucesso!");
  }

  fecharModalCorrecao(); // fecha o modal
  loadDashboard();       // recarrega o painel para refletir as mudanças
}


// ─────────────────────────────────────────────────────────────
// gerarCertificadoDoPainel(courseTitle)
// Abre o modal de certificado para um curso concluído.
// Usa a função generateCertificate() de certificate.js se
// disponível, senão faz o fallback preenchendo o modal manualmente.
// ─────────────────────────────────────────────────────────────
function gerarCertificadoDoPainel(courseTitle) {
  if (typeof generateCertificate === "function") {
    // Delegado para certificate.js (método preferencial)
    generateCertificate(courseTitle);
  } else {
    // Fallback: preenche e abre o modal de certificado diretamente
    const user = JSON.parse(localStorage.getItem("loggedUser"));
    if(document.getElementById("certUser"))   document.getElementById("certUser").innerText   = user.name;
    if(document.getElementById("certCourse")) document.getElementById("certCourse").innerText = courseTitle;
    if(document.getElementById("certDate"))   document.getElementById("certDate").innerText   = new Date().toLocaleDateString('pt-BR');
    if(document.getElementById("certificateModal")) document.getElementById("certificateModal").style.display = "flex";
  }
}


// ─────────────────────────────────────────────────────────────
// Inicialização automática
// Quando o HTML da página terminar de carregar, chama loadDashboard()
// para renderizar o painel do usuário logado (se houver).
// ─────────────────────────────────────────────────────────────
document.addEventListener("DOMContentLoaded", loadDashboard);


