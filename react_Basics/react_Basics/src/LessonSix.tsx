import { useEffect, useState } from "react";

type LessonSixProps = {
  topic: string;
};

// This lesson teaches how to use useEffect for side effects.
export function LessonSix({ topic }: LessonSixProps) {
  // -----------------------------------------
  // 1. RUN CODE ON COMPONENT MOUNT
  // -----------------------------------------
  useEffect(() => {
    console.log("LessonSix mounted!");
  }, []); 
  // Empty dependency array [] = run only once when component loads


  // -----------------------------------------
  // 2. RUN CODE WHEN STATE CHANGES
  // -----------------------------------------
  const [count, setCount] = useState(0);

  useEffect(() => {
    console.log("Count changed:", count);
  }, [count]);  
  // Runs every time `count` changes


  // -----------------------------------------
  // 3. FETCH DATA FROM AN API (Fake API)
  // -----------------------------------------
  const [title, setTitle] = useState("");

  useEffect(() => {
    // A simple fetch call to a testing API
    fetch("https://jsonplaceholder.typicode.com/todos/1")    // fetch() returns a Promise = "Hey server, give me the data. I'll continue later."
      .then((res) => res.json())                             // res.json() also returns a Promise.
      .then((data) => {
        console.log("Fetched data:", data);
        setTitle(data.title);                                // Store the title in our React state.
      });
  }, []);
            // [] = dependency array
            // Empty array means:
            //  - run once on mount
            //  - do NOT run again, ever (unless component is recreated)


  // -----------------------------------------
  // 4. CLEANUP EXAMPLE (Unmount)
  // -----------------------------------------
  useEffect(() => {
    console.log("Setting up interval...");

    const id = setInterval(() => {                          // setInterval runs a function every 2 seconds.
      console.log("Interval tick");                         // React does NOT automatically clean up intervals.
    }, 2000);                                               // Without cleanup, we would create memory leaks.

    // ------------------------------------------------------
    // RETURN FUNCTION = cleanup function
    // React calls this when the component UNMOUNTS.
    //
    // Example of unmounting:
    //  - The user navigates away
    //  - Component disappears from the page
    //  - Parent stops rendering this component
    //
    // Cleanup is critical to avoid:
    //  - intervals running in background
    //  - event listeners stacking
    //  - memory leaks
    // ------------------------------------------------------
    return () => {
      console.log("Cleaning up interval...");
      clearInterval(id);
    };
  }, []);

    // Again, empty dependency array means:
    //  - set up the interval once
    //  - cleanup only when unmounting

  return (
    <section>
      <h2>Lesson 6: {topic}</h2>

      {/* Counter */}
      <h3>Counter with useEffect</h3>
      <p>Count: {count}</p>
      <button onClick={() => setCount(count + 1)}>Increase</button>

      {/* API result */}
      <h3 style={{ marginTop: "1rem" }}>Fetched Title:</h3>
      <p>{title || "Loading..."}</p>

      <p style={{ marginTop: "1rem", fontSize: "0.9rem", opacity: 0.7 }}>
        Open your browser console to see useEffect logs.
      </p>
    </section>
  );
}
