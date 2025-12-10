import React, { useCallback, useMemo, useState } from "react";

type LessonThirteenProps = {
  topic: string;
};

/* --------------------------------------------------------
   Child component to demonstrate React.memo
-------------------------------------------------------- */
const Child = React.memo(function Child({ onClick }: { onClick: () => void }) {
  console.log("%cChild rendered", "color: orange");

  return (
    <button onClick={onClick} style={{ marginTop: "1rem" }}>
      Child Button
    </button>
  );
});

/* --------------------------------------------------------
   Main lesson component
-------------------------------------------------------- */
export function LessonThirteen({ topic }: LessonThirteenProps) {
  const [count, setCount] = useState(0);
  const [value, setValue] = useState("");

  /* --------------------------------------------------------
     1. EXPENSIVE CALCULATION EXAMPLE
        Without useMemo this runs on EVERY re-render!
  -------------------------------------------------------- */

  function simulateSlowOperation() {
    console.log("%cSimulating slow calculation...", "color: red");

    let total = 0;
    for (let i = 0; i < 1_000_000_00; i++) {
      total += i;
    }

    return total;
  }

  // useMemo caches the result until dependencies change
  const slowResult = useMemo(() => simulateSlowOperation(), []); //Cached, Runs once        Use When: Heavy Loops, Expensive tranformations, Sorting large lists, Filtering large lists

  /* --------------------------------------------------------
     2. Function passed to a child component
        Without useCallback, this function is recreated on
        every render -> Child rerenders unnecessarily.
  -------------------------------------------------------- */

  const handleChildClick = useCallback(() => {
    console.log("Child clicked!");
  }, []); // stable reference

  return (
    <section>
      <h2>Lesson 13: {topic}</h2>

      {/* Counter to force re-renders */}
      <p>Count: {count}</p>
      <button onClick={() => setCount(count + 1)}>Increase</button>

      {/* Input that causes rerenders */}
      <div style={{ marginTop: "1rem" }}>
        <input
          placeholder="Type something..."
          value={value}
          onChange={(e) => setValue(e.target.value)}
        />
      </div>

      {/* Expensive calculation */}
      <h3 style={{ marginTop: "1rem" }}>Expensive calculation result:</h3>
      <p>{slowResult}</p>

      {/* Child component */}
      <Child onClick={handleChildClick} />
    </section>
  );
}
