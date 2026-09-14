const prisma = require("../lib/prisma");
const { applyLevelUps } = require("../lib/xp");
const { toPublicUser } = require("./authService");
const ApiError = require("../lib/ApiError");

// Recompensas menores de gamificação (meta concluída, treino rápido de
// questões) — a mesma matemática de nível de upsertProgress, mas para XP
// que não vem da conclusão de um curso. O tamanho do prêmio é decidido
// pelo cliente (ex: "+50 por meta", "+25 por questão"), então limitamos
// o valor aceito por chamada para conter o abuso via chamada direta à API.
const MAX_XP_PER_CALL = 100;

async function awardXp(userId, amount) {
  const safeAmount = Math.max(1, Math.min(MAX_XP_PER_CALL, Math.floor(Number(amount) || 0)));
  if (safeAmount <= 0) {
    throw new ApiError(400, "Quantidade de XP inválida.");
  }

  const user = await prisma.user.findUnique({ where: { id: userId } });
  const result = applyLevelUps({ xp: user.xp + safeAmount, level: user.level });

  const updated = await prisma.user.update({
    where: { id: userId },
    data: { xp: result.xp, level: result.level },
  });

  return { user: toPublicUser(updated), leveledUp: result.leveledUp };
}

module.exports = { awardXp };
