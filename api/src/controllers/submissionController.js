const asyncHandler = require("../lib/asyncHandler");
const submissionService = require("../services/submissionService");

const create = asyncHandler(async (req, res) => {
  const submission = await submissionService.createOrReplace(req.user.userId, req.body);
  res.status(201).json(submission);
});

const listForTeacher = asyncHandler(async (req, res) => {
  const submissions = await submissionService.listForTeacher(req.query);
  res.json(submissions);
});

const listMine = asyncHandler(async (req, res) => {
  const submissions = await submissionService.listMine(req.user.userId, req.query);
  res.json(submissions);
});

const correct = asyncHandler(async (req, res) => {
  const submission = await submissionService.correct(req.params.id, req.body);
  res.json(submission);
});

module.exports = { create, listForTeacher, listMine, correct };
