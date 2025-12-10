import { useTheme } from "./ThemeContext";

type LessonTenProps = {
  topic: string;
};

export function LessonTen({ topic }: LessonTenProps) {
  const { theme, toggleTheme } = useTheme(); 
  // Access global theme + toggle function

  return (
    <section>
      <h2>Lesson 10: {topic}</h2>

      <p>Current theme: <strong>{theme}</strong></p>

      <button onClick={toggleTheme}>
        Toggle Theme
      </button>

      <p style={{ marginTop: "1rem" }}>
        This component reads global state.
      </p>
    </section>
  );
}
