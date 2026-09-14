const { Router } = require("express");
const goalController = require("../controllers/goalController");
const { requireAuth } = require("../middleware/auth");

const router = Router();

router.use(requireAuth);

router.get("/", goalController.list);
router.post("/", goalController.create);
router.patch("/:id", goalController.update);
router.delete("/:id", goalController.remove);

module.exports = router;
