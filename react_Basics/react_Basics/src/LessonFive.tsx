import { useState } from "react";

type LessonFiveProps = {
  topic: string;
};

// -----------------------------------------------
// Child Component
// -----------------------------------------------
type ChildProps = {
  // The parent will pass this text to the child
  message: string;

  // The parent will pass a callback function
  onButtonClick: () => void;
};

function ChildComponent({ message, onButtonClick }: ChildProps) {
  return (
    <div style={{ border: "1px solid #ddd", padding: "1rem", marginTop: "1rem" }}>
      <h4>Child Component</h4>

      {/* This text comes FROM the parent */}
      <p>Message from parent: {message}</p>

      {/* This button calls a function FROM the parent */}
      <button onClick={onButtonClick}>Click to notify parent</button>
    </div>
  );
}

// -----------------------------------------------
// Parent Component (LessonFive)
// -----------------------------------------------
export function LessonFive({ topic }: LessonFiveProps) {
  // Parent state
  const [parentMessage, setParentMessage] = useState("Hello from Parent!");
  const [childClicks, setChildClicks] = useState(0);

  // Parent function to be called by the child component
  function handleChildClick() {
    setChildClicks(childClicks + 1);
  }

  function updateMessage() {
  setParentMessage("The parent message changed!");
}

  return (
    <section>
      <h2>Lesson 5: {topic}</h2>

       <button onClick={updateMessage}>Change Parent Message</button>

      {/* Display the number of times child notified the parent */}
      <p>Child has clicked the button {childClicks} times.</p>

      {/* Pass props (data + callback) DOWN into the child component */}
      <ChildComponent 
        message={parentMessage}
        onButtonClick={handleChildClick}
      />
    </section>
  );
}
