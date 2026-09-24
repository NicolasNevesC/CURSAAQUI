// ============================================================
// storage.js - Camada única de acesso ao localStorage
//
// Antes, cada tela guardava o usuário logado duas vezes:
// uma cópia inteira em "loggedUser" e outra dentro do array
// "users". Toda alteração (xp, nível, nome...) precisava ser
// escrita nos dois lugares manualmente, e várias telas
// esqueciam de sincronizar um dos dois.
//
// Agora "users" é a única fonte de verdade dos dados do usuário.
// A sessão ativa guarda apenas o e-mail de quem está logado
// (LOGGED_USER_EMAIL) e os dados completos são sempre lidos a
// partir da lista de usuários.
// ============================================================

const Storage = {
  KEYS: {
    USERS: "users",
    LOGGED_USER_EMAIL: "loggedUserEmail",
    SELECTED_COURSE: "selectedCourse"
  },

  getUsers() {
    return JSON.parse(localStorage.getItem(this.KEYS.USERS)) || [];
  },

  setUsers(users) {
    localStorage.setItem(this.KEYS.USERS, JSON.stringify(users));
  },

  findUserByEmail(email) {
    if (!email) return null;
    return this.getUsers().find(u => u.email === email) || null;
  },

  // Insere o usuário na lista ou substitui o existente (mesmo e-mail)
  upsertUser(user) {
    const users = this.getUsers();
    const idx = users.findIndex(u => u.email === user.email);
    if (idx !== -1) {
      users[idx] = user;
    } else {
      users.push(user);
    }
    this.setUsers(users);
  },

  // Migra sessões antigas que guardavam o usuário inteiro em "loggedUser"
  _migrateLegacyLoggedUser() {
    const legacy = localStorage.getItem("loggedUser");
    if (legacy === null) return;
    try {
      const legacyUser = JSON.parse(legacy);
      if (legacyUser && legacyUser.email) {
        this.upsertUser(legacyUser);
        localStorage.setItem(this.KEYS.LOGGED_USER_EMAIL, legacyUser.email);
      }
    } catch (e) {
      // sessão antiga corrompida: ignora e descarta
    }
    localStorage.removeItem("loggedUser");
  },

  getLoggedUser() {
    this._migrateLegacyLoggedUser();
    const email = localStorage.getItem(this.KEYS.LOGGED_USER_EMAIL);
    return email ? this.findUserByEmail(email) : null;
  },

  // Define quem está logado. O objeto do usuário é sempre gravado
  // (ou atualizado) dentro de "users" — nunca duplicado à parte.
  setLoggedUser(user) {
    if (!user) {
      this.clearLoggedUser();
      return;
    }
    this.upsertUser(user);
    localStorage.setItem(this.KEYS.LOGGED_USER_EMAIL, user.email);
  },

  // Aplica um patch de campos (ex.: { xp, level }) no usuário logado
  // e persiste a mudança no único local onde ele existe.
  updateLoggedUser(patch) {
    const user = this.getLoggedUser();
    if (!user) return null;
    Object.assign(user, patch);
    this.setLoggedUser(user);
    return user;
  },

  clearLoggedUser() {
    localStorage.removeItem(this.KEYS.LOGGED_USER_EMAIL);
  },

  getSelectedCourse() {
    return JSON.parse(localStorage.getItem(this.KEYS.SELECTED_COURSE));
  },

  setSelectedCourse(course) {
    localStorage.setItem(this.KEYS.SELECTED_COURSE, JSON.stringify(course));
  },

  clearSelectedCourse() {
    localStorage.removeItem(this.KEYS.SELECTED_COURSE);
  }
};

window.Storage = Storage;
