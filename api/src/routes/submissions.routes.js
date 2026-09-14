const { Router } = require("express");
const submissionController = require("../controllers/submissionController");
const { requireAuth } = require("../middleware/auth");
const requireRole = require("../middleware/requireRole");

const router = Router();

router.use(requireAuth);

router.post("/", requireRole("STUDENT"), submissionController.create);
router.get("/mine", requireRole("STUDENT"), submissionController.listMine);
router.get("/", requireRole("TEACHER"), submissionController.listForTeacher);
router.patch("/:id", requireRole("TEACHER"), submissionController.correct);

module.exports = router;
