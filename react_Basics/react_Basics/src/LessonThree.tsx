type LessonThreeProps = {
  topic: string;
};

// Component demonstrating event handling in React.
export function LessonThree({ topic }: LessonThreeProps) {
  
  // A simple function that runs when a button is clicked.
  function handleClick() {
    console.log("Button clicked!");
    alert("You clicked the button!"); // For visible feedback in the browser
  }

  // Function with a parameter — shows how to pass custom data.
  function greetUser(name: string) {
    alert(`Hello, ${name}!`);
  }

  // Event handler for input fields.
  // 'e' is a synthetic React event (typed automatically).
  function handleInputChange(e: React.ChangeEvent<HTMLInputElement>) {
    console.log("Input changed:", e.target.value);
  }

  return (
    <section>
      <h2>Lesson 3: {topic}</h2>

      {/* Simple button click event */}
      <button onClick={handleClick}>
        Click Me
      </button>

      {/* Button that passes an argument to a function */}
      <button onClick={() => greetUser("UserXXX")}>
        Greet User
      </button>

      {/* Input field event: fires on every text change */}
      <div style={{ marginTop: "1rem" }}>
        <input 
          type="text"
          placeholder="Type something..."
          onChange={handleInputChange}
        />
      </div>
    </section>
  );
}
