import { LessonOne } from "./LessonOne";
import { LessonTwo } from "./LessonTwo";
import { LessonThree } from "./LessonThree";
import { LessonFour } from "./LessonFour";
import { LessonFive } from "./LessonFive";
import { LessonSix } from "./LessonSix";
import { LessonSeven } from "./LessonSeven";
import { LessonEight } from "./LessonEight";
import { LessonNine } from "./LessonNine";
import { LessonTen } from "./LessonTen";
import { LessonEleven } from "./LessonEleven";
import { LessonTwelve } from "./LessonTwelve";
import { LessonThirteen } from "./LessonThirteen";

import { Route, Routes, Link } from "react-router-dom";

// Main React component of the entire application.
// — the entry point of UI.
function App() {
  return (
    <div>
     <h1>React Basics Playground</h1>

      {/* Navigation Menu */}
      <nav style={{ marginBottom: "1rem" }}>
        <Link to="/" style={{ marginRight: "1rem" }}>Home</Link>
        <Link to="/lesson1" style={{ marginRight: "1rem" }}>Lesson 1</Link>
        <Link to="/lesson2" style={{ marginRight: "1rem" }}>Lesson 2</Link>
        <Link to="/lesson3" style={{ marginRight: "1rem" }}>Lesson 3</Link>
        <Link to="/lesson4" style={{ marginRight: "1rem" }}>Lesson 4</Link>
        <Link to="/lesson5" style={{ marginRight: "1rem" }}>Lesson 5</Link>
        <Link to="/lesson6" style={{ marginRight: "1rem" }}>Lesson 6</Link>
        <Link to="/lesson7" style={{ marginRight: "1rem" }}>Lesson 7</Link>
        <Link to="/lesson8" style={{ marginRight: "1rem" }}>Lesson 8</Link>
        <Link to="/lesson9" style={{ marginRight: "1rem" }}>Lesson 9</Link>
        <Link to="/lesson10" style={{ marginRight: "1rem" }}>Lesson 10</Link>
        <Link to="/lesson11" style={{ marginRight: "1rem" }}>Lesson 11</Link>
        <Link to="/lesson12" style={{ marginRight: "1rem" }}>Lesson 12</Link>
        <Link to="/lesson13">Lesson 13</Link>
      </nav>

      {/* Routes decide WHICH component to show based on URL */}
      <Routes>
        <Route path="/" element={<p>Welcome! Choose a lesson above.</p>} />
        <Route path="/lesson1" element={<LessonOne topic="Components and JSX" />} />
        <Route path="/lesson2" element={<LessonTwo topic="Rendering and JSX Expressions" />} />
        <Route path="/lesson3" element={<LessonThree topic="Events and User Interaction" />} />
        <Route path="/lesson4" element={<LessonFour topic="State and Dynamic UI (useState)" />} />
        <Route path="/lesson5" element={<LessonFive topic="Parent <-> Child Communication" />} />
        <Route path="/lesson6" element={<LessonSix topic="useEffect — Side Effects" />} />
        <Route path="/lesson7" element={<LessonSeven topic="Dynamic Lists" />} />
        <Route path="/lesson8" element={<LessonEight topic="Forms & Validation" />} />
        <Route path="/lesson9" element={<LessonNine topic="Styling with CSS Modules" />} />
        <Route path="/lesson10" element={<LessonTen topic="Context API (Global State)" />} />
        <Route path="/lesson11" element={<LessonEleven topic="Custom Hooks" />} />
        <Route path="/lesson12" element={<LessonTwelve topic="Error Boundaries — UI Crash Protection" />} />
        <Route path="/lesson13" element={<LessonThirteen topic="Memoization & Performance Optimization" />} />
      </Routes>
    </div>
  );
}

// Export the component so other files can import and use it.
export default App;