type LessonTwoProps = {
  topic: string;
};

// This component demonstrates JSX basics: variables, expressions,
// conditional rendering, and rendering lists.
export function LessonTwo({ topic }: LessonTwoProps) {
  
  //TypeScript string variable
  const studentName = "User";

  //TypeScript number variable
  const lessonsCompleted = 1;

  //boolean for conditional rendering
  const isLearning = true;

  //array of strings to render dynamically
  const topics = ["JSX", "Components", "Props", "State"];

  return (
    // Parent JSX element required — React components must return ONE root element
    <section>
      {/* Display the topic passed from props */}
      <h2>Lesson 2: {topic}</h2>

      {/* Rendering a string variable */}
      <p>Hello {studentName}, welcome back!</p>

      {/* Rendering an expression — JSX can evaluate JS/TS inside {} */}
      <p>You have completed {lessonsCompleted * 2} mini-lessons.</p>

      {/* Conditional rendering:
          If isLearning == true, React renders the <p> element.
          If false, React skips it entirely. */}
      {isLearning && <p>Great! You're actively learning React</p>}

      {/* Rendering arrays using .map():
          For each element 't' in topics, return a <li>.
          We must always include a unique 'key'. */}
      <h3>Topics we will cover today:</h3>
      <ul>
        {topics.map((t, index) => (
          <li key={index}>{t}</li>
        ))}
      </ul>
    </section>
  );
}
