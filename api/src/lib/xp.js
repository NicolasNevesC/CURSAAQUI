// Compartilhado entre progressService (conclusão de curso) e userService
// (recompensas menores como meta concluída / treino rápido) para manter
// a mesma matemática de subida de nível em um único lugar.
function applyLevelUps({ xp, level }) {
  let nextXp = xp;
  let nextLevel = level;
  let leveledUp = false;

  let xpNeeded = nextLevel * 1000;
  while (nextXp >= xpNeeded) {
    nextXp -= xpNeeded;
    nextLevel += 1;
    leveledUp = true;
    xpNeeded = nextLevel * 1000;
  }

  return { xp: nextXp, level: nextLevel, leveledUp };
}

module.exports = { applyLevelUps };
