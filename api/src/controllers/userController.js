const asyncHandler = require("../lib/asyncHandler");
const userService = require("../services/userService");

const awardXp = asyncHandler(async (req, res) => {
  const result = await userService.awardXp(req.user.userId, req.body.amount);
  res.json(result);
});

module.exports = { awardXp };
