// Define the shape (type) of props that this component expects.
// Similar to defining a class constructor with parameters in C#.
type LessonOneProps = {
  topic: string;
};

// The LessonOne component.
// It receives props (here destructured into { topic }) and returns JSX.
// JSX is React's way to combine HTML-like tags with JavaScript.
export function LessonOne({ topic }: LessonOneProps) {
  return (
    <section>
      <h2>Lesson 1: {topic}</h2>
      <p>This is my first custom React component.</p>
    </section>
  );
}
