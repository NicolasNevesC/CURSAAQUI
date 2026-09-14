const asyncHandler = require("../lib/asyncHandler");
const reviewService = require("../services/reviewService");

const listForCourse = asyncHandler(async (req, res) => {
  res.json(await reviewService.listForCourse(req.params.courseId));
});

const create = asyncHandler(async (req, res) => {
  const review = await reviewService.create({
    courseId: req.params.courseId,
    userId: req.user ? req.user.userId : null,
    rating: req.body.rating,
    comment: req.body.comment,
  });
  res.status(201).json(review);
});

module.exports = { listForCourse, create };
