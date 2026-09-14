require("dotenv").config();

const express = require("express");
const cors = require("cors");

const errorHandler = require("./middleware/errorHandler");
const authRoutes = require("./routes/auth.routes");
const coursesRoutes = require("./routes/courses.routes");
const progressRoutes = require("./routes/progress.routes");
const submissionsRoutes = require("./routes/submissions.routes");
const goalsRoutes = require("./routes/goals.routes");
const notesRoutes = require("./routes/notes.routes");
const reviewsRoutes = require("./routes/reviews.routes");
const usersRoutes = require("./routes/users.routes");

const app = express();

app.use(cors({ origin: process.env.CORS_ORIGIN || "*" }));
app.use(express.json());

app.get("/health", (req, res) => res.json({ status: "ok" }));

app.use("/auth", authRoutes);
app.use("/courses", coursesRoutes);
app.use("/progress", progressRoutes);
app.use("/submissions", submissionsRoutes);
app.use("/goals", goalsRoutes);
app.use("/notes", notesRoutes);
app.use("/reviews", reviewsRoutes);
app.use("/users", usersRoutes);

app.use((req, res) => res.status(404).json({ message: "Rota não encontrada." }));
app.use(errorHandler);

const PORT = process.env.PORT || 3001;
app.listen(PORT, () => {
  console.log(`API Cursa Aqui rodando em http://localhost:${PORT}`);
});
