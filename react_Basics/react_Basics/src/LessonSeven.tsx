import { useState } from "react";

type LessonSevenProps = {
  topic: string;
};

export function LessonSeven({ topic }: LessonSevenProps) {
  // -------------------------------------------------------------
  // 1. LIST STATE — an array of strings
  // -------------------------------------------------------------
  const [items, setItems] = useState<string[]>([
    "Learn JSX",
    "Learn Components",
    "Learn State",
  ]);

  // -------------------------------------------------------------
  // 2. TEMP VALUE FOR THE INPUT FIELD
  // -------------------------------------------------------------
  const [newItem, setNewItem] = useState("");

  // Add new item to the list
  function addItem() {
    if (newItem.trim() === "") return; // prevent empty items

    // Create a NEW array (React requires immutability)
    setItems([...items, newItem]);

    // Clear input after adding
    setNewItem("");
  }

  // Remove an item by index
  function removeItem(index: number) {
    // Filter returns a new array, excluding the removed item
    const updated = items.filter((_, i) => i !== index);
    setItems(updated);
  }

  return (
    <section>
      <h2>Lesson 7: {topic}</h2>

      {/* ---------------- Add Item UI ---------------- */}
      <div style={{ marginBottom: "1rem" }}>
        <input
          type="text"
          placeholder="New item..."
          value={newItem}
          onChange={(e) => setNewItem(e.target.value)}
        />

        <button style={{ marginLeft: "0.5rem" }} onClick={addItem}>
          Add
        </button>
      </div>

      {/* ---------------- List Rendering ---------------- */}
      <h3>Items:</h3>
      <ul>
        {items.map((item, index) => (
          // KEY is extremely important! It helps React track each item.
          // index is acceptable for simple lists (not ideal for big apps)
          <li key={index} style={{ marginBottom: "0.5rem" }}>
            {item}

            {/* Remove button */}
            <button
              onClick={() => removeItem(index)}
              style={{ marginLeft: "1rem" }}
            >
              Remove
            </button>
          </li>
        ))}
      </ul>
    </section>
  );
}
