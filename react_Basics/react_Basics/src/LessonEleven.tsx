import { useToggle } from "./hooks/useToggle";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { useFetch } from "./hooks/useFetch";

type LessonElevenProps = {
  topic: string;
};

export function LessonEleven({ topic }: LessonElevenProps) {
  // Toggle example
  const [isOpen, toggleOpen] = useToggle();

  // LocalStorage example
  const [username, setUsername] = useLocalStorage("username", "");

  // Fetch example
  const { data, loading, error } = useFetch<{ title: string }>(
    "https://jsonplaceholder.typicode.com/todos/1"
  );

  return (
    <section>
      <h2>Lesson 11: {topic}</h2>

      {/* Toggle Example */}
      <h3>useToggle Example</h3>
      <button onClick={toggleOpen}>{isOpen ? "Hide" : "Show"}</button>
      {isOpen && <p>This text is toggled on/off!</p>}

      {/* LocalStorage Example */}
      <h3 style={{ marginTop: "1rem" }}>useLocalStorage Example</h3>
      <input
        placeholder="Enter username..."
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />
      <p>Saved username: {username}</p>

      {/* Fetch Example */}
      <h3 style={{ marginTop: "1rem" }}>useFetch Example</h3>
      {loading && <p>Loading...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}
      {data && <p>Fetched Title: {data.title}</p>}
    </section>
  );
}
