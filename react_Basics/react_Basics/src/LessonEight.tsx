import { useState } from "react";

type LessonEightProps = {
  topic: string;
};

// Interface to describe form data structure
interface FormData {
  name: string;
  age: number;
  role: string;
  notes: string;
}

export function LessonEight({ topic }: LessonEightProps) {
  // ------------------------------------------------------------
  // FORM STATE
  // One object holding all form input values
  // ------------------------------------------------------------
  const [form, setForm] = useState<FormData>({
    name: "",
    age: 18,
    role: "User",
    notes: "",
  });

  // ------------------------------------------------------------
  // STATE FOR ERRORS & SUBMITTED DATA
  // ------------------------------------------------------------
  const [error, setError] = useState("");
  const [submittedData, setSubmittedData] = useState<FormData | null>(null);

  // ------------------------------------------------------------
  // HANDLE TEXT / NUMBER / TEXTAREA CHANGES
  //
  // This function updates ANY field based on the name="" attribute
  // e.g., <input name="name" ...> updates form.name
  // ------------------------------------------------------------
  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) {
    const { name, value } = e.target;

    // Update the form state dynamically
    setForm({
      ...form, //spread operator -> Copies ALL existing fields of the form object -> Without spreading, we'd LOSE all other fields.
      [name]: name === "age" ? Number(value) : value,                                                                                       // [name] means: Use the value of the variable "name" as the property key. (instead of writing 4 different handlers)
    });                                                                                                                                     // HTML <input type="number"> returns STRING values by default, so we convert it using Number().
  }

  // ------------------------------------------------------------
  // FORM SUBMIT HANDLER
  // ------------------------------------------------------------
  function handleSubmit(e: React.FormEvent) {
    e.preventDefault(); // Prevent page refresh

    // Simple validation
    if (form.name.trim() === "") {
      setError("Name is required.");
      return;
    }
    if (form.age < 1) {
      setError("Age must be positive.");
      return;
    }

    // If valid, clear error and save submission
    setError("");
    setSubmittedData(form);
  }

  return (
    <section>
      <h2>Lesson 8: {topic}</h2>

      {/* --------------------------------------------------------
          FORM UI
         -------------------------------------------------------- */}
      <form onSubmit={handleSubmit}>
        {/* Name */}
        <div style={{ marginBottom: "1rem" }}>
          <label>Name: </label>
          <input
            type="text"
            name="name"       // connects the field to form.name
            value={form.name} // controlled input
            onChange={handleChange}
          />
        </div>

        {/* Age */}
        <div style={{ marginBottom: "1rem" }}>
          <label>Age: </label>
          <input
            type="number"
            name="age"
            value={form.age}
            onChange={handleChange}
          />
        </div>

        {/* Role (select dropdown) */}
        <div style={{ marginBottom: "1rem" }}>
          <label>Role: </label>
          <select name="role" value={form.role} onChange={handleChange}>
            <option value="User">User</option>
            <option value="Admin">Admin</option>
            <option value="Moderator">Moderator</option>
          </select>
        </div>

        {/* Notes */}
        <div style={{ marginBottom: "1rem" }}>
          <label>Notes: </label>
          <textarea
            name="notes"
            value={form.notes}
            onChange={handleChange}
            rows={3}
          />
        </div>

        {/* Submit button */}
        <button type="submit">Submit</button>

        {/* Validation error */}
        {error && (
          <p style={{ color: "red", marginTop: "0.5rem" }}>
            Error {error}
          </p>
        )}
      </form>

      {/* --------------------------------------------------------
          DISPLAY SUBMITTED DATA
         -------------------------------------------------------- */}
      {submittedData && (
        <div style={{ marginTop: "2rem", borderTop: "1px solid #ccc", paddingTop: "1rem" }}>
          <h3>Submitted Data:</h3>
          <p><strong>Name:</strong> {submittedData.name}</p>
          <p><strong>Age:</strong> {submittedData.age}</p>
          <p><strong>Role:</strong> {submittedData.role}</p>
          <p><strong>Notes:</strong> {submittedData.notes}</p>
        </div>
      )}
    </section>
  );
}
