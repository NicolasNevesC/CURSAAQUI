const asyncHandler = require("../lib/asyncHandler");
const noteService = require("../services/noteService");

const get = asyncHandler(async (req, res) => {
  const content = await noteService.get(req.user.userId, req.params.courseId);
  res.json({ content });
});

const upsert = asyncHandler(async (req, res) => {
  const note = await noteService.upsert(req.user.userId, req.params.courseId, req.body.content || "");
  res.json(note);
});

module.exports = { get, upsert };
