const { Router } = require("express");
const reviewController = require("../controllers/reviewController");
const { optionalAuth } = require("../middleware/auth");

const router = Router();

router.get("/:courseId", reviewController.listForCourse);
router.post("/:courseId", optionalAuth, reviewController.create);

module.exports = router;
