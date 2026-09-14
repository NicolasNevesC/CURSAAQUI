const { Router } = require("express");
const progressController = require("../controllers/progressController");
const { requireAuth } = require("../middleware/auth");
const requireRole = require("../middleware/requireRole");

const router = Router();

router.use(requireAuth, requireRole("STUDENT"));

router.get("/", progressController.getAll);
router.get("/:courseId", progressController.getOne);
router.put("/:courseId", progressController.upsert);

module.exports = router;
