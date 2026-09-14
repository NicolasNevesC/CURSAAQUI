const asyncHandler = require("../lib/asyncHandler");
const goalService = require("../services/goalService");

const list = asyncHandler(async (req, res) => {
  res.json(await goalService.list(req.user.userId));
});

const create = asyncHandler(async (req, res) => {
  res.status(201).json(await goalService.create(req.user.userId, req.body.text));
});

const update = asyncHandler(async (req, res) => {
  res.json(await goalService.update(req.user.userId, req.params.id, req.body));
});

const remove = asyncHandler(async (req, res) => {
  await goalService.remove(req.user.userId, req.params.id);
  res.status(204).send();
});

module.exports = { list, create, update, remove };
