const { Router } = require("express");
const userController = require("../controllers/userController");
const { requireAuth } = require("../middleware/auth");
const requireRole = require("../middleware/requireRole");

const router = Router();

router.post("/me/xp", requireAuth, requireRole("STUDENT"), userController.awardXp);

module.exports = router;
