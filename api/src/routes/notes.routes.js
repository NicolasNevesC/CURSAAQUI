const { Router } = require("express");
const noteController = require("../controllers/noteController");
const { requireAuth } = require("../middleware/auth");

const router = Router();

router.use(requireAuth);

router.get("/:courseId", noteController.get);
router.put("/:courseId", noteController.upsert);

module.exports = router;
