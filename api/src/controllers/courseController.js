const asyncHandler = require("../lib/asyncHandler");
const courseService = require("../services/courseService");

const list = asyncHandler(async (req, res) => {
  const courses = await courseService.listCourses(req.query);
  res.json(courses);
});

const getById = asyncHandler(async (req, res) => {
  const course = await courseService.getCourseById(req.params.id);
  res.json(course);
});

module.exports = { list, getById };
