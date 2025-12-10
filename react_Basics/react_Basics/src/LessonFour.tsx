import { useState } from "react";

type LessonFourProps = {
  topic: string;
};

// This lesson teaches useState — the most important React hook.
export function LessonFour({ topic }: LessonFourProps) {

  // ----------------------------
  // 1. COUNTER EXAMPLE
  // ----------------------------
  // useState creates a state variable (`count`) and a function to update it (`setCount`).
  // The initial value is 0.
  const [count, setCount] = useState(0);

  function increase() {
    // Updating state triggers a re-render (UI updates automatically)
    setCount(count + 1);
  }

  // ----------------------------
  // 2. LIVE INPUT EXAMPLE
  // ----------------------------
  // State holding text typed by the user.
  const [text, setText] = useState("");

  function handleTextChange(e: React.ChangeEvent<HTMLInputElement>) {
    setText(e.target.value); // updates state with new text
  }

  // ----------------------------
  // 3. TOGGLE EXAMPLE
  // ----------------------------
  // State boolean for show/hide
  const [visible, setVisible] = useState(true);

  function toggleVisibility() {
    setVisible(!visible);
  }

  return (
    <section>
      <h2>Lesson 4: {topic}</h2>

      {/* ---------------- Counter UI ---------------- */}
      <h3>Counter Example</h3>
      <p>Count: {count}</p>
      <button onClick={increase}>Increase</button>

      {/* ---------------- Live Input UI ---------------- */}
      <h3 style={{ marginTop: "1rem" }}>Live Input Example</h3>
      <input
        type="text"
        placeholder="Type something..."
        onChange={handleTextChange}
      />
      <p>You typed: {text}</p>

      {/* ---------------- Toggle UI ---------------- */}
      <h3 style={{ marginTop: "1rem" }}>Toggle Example</h3>
      <button onClick={toggleVisibility}>Toggle Message</button>

      {/* Only render paragraph if visible === true */}
      {visible && <p>This message can be hidden or shown.</p>}
    </section>
  );
}
