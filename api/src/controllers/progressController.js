const asyncHandler = require("../lib/asyncHandler");
const progressService = require("../services/progressService");

const getAll = asyncHandler(async (req, res) => {
  const map = await progressService.getAllForUser(req.user.userId);
  res.json(map);
});

const getOne = asyncHandler(async (req, res) => {
  const record = await progressService.getOne(req.user.userId, req.params.courseId);
  res.json(record || null);
});

const upsert = asyncHandler(async (req, res) => {
  const result = await progressService.upsertProgress(req.user.userId, req.params.courseId, req.body);
  res.json(result);
});

module.exports = { getAll, getOne, upsert };
