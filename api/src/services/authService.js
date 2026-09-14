const bcrypt = require("bcryptjs");
const prisma = require("../lib/prisma");
const { signToken } = require("../lib/jwt");
const ApiError = require("../lib/ApiError");

function toPublicUser(user) {
  const { password, ...publicUser } = user;
  return publicUser;
}

async function register({ name, email, password, role }) {
  if (!name || !email || !password) {
    throw new ApiError(400, "Preencha todos os campos!");
  }

  const existing = await prisma.user.findUnique({ where: { email } });
  if (existing) {
    throw new ApiError(409, "Este e-mail já está cadastrado.");
  }

  const normalizedRole = role === "teacher" || role === "TEACHER" ? "TEACHER" : "STUDENT";
  const passwordHash = await bcrypt.hash(password, 10);

  const user = await prisma.user.create({
    data: { name, email, password: passwordHash, role: normalizedRole, xp: 0, level: 1 },
  });

  return { token: signToken(user), user: toPublicUser(user) };
}

async function login({ email, password }) {
  const user = await prisma.user.findUnique({ where: { email } });
  if (!user) {
    throw new ApiError(401, "Usuário ou senha incorretos.");
  }

  const matches = await bcrypt.compare(password, user.password);
  if (!matches) {
    throw new ApiError(401, "Usuário ou senha incorretos.");
  }

  return { token: signToken(user), user: toPublicUser(user) };
}

async function me(userId) {
  const user = await prisma.user.findUnique({ where: { id: userId } });
  if (!user) {
    throw new ApiError(404, "Usuário não encontrado.");
  }
  return toPublicUser(user);
}

module.exports = { register, login, me, toPublicUser };
