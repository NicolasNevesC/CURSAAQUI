const { Router } = require("express");
const courseController = require("../controllers/courseController");
const { optionalAuth } = require("../middleware/auth");

const router = Router();

router.get("/", optionalAuth, courseController.list);
router.get("/:id", optionalAuth, courseController.getById);

module.exports = router;
