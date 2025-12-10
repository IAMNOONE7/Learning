import styles from "./LessonNine.module.css"; 
// Importing CSS Module. `styles` now contains class names that are unique.

type LessonNineProps = {
  topic: string;
};

export function LessonNine({ topic }: LessonNineProps) {
  return (
    <section className={styles.card}>
      {/* Title styled from CSS module */}
      <h2 className={styles.title}>Lesson 9: {topic}</h2>

      <p>
        This lesson introduces <strong>CSS Modules</strong>, 
        a great way to scope styles in React and avoid global CSS conflicts.
      </p>

      <p>Below is an example of a button styled using CSS Modules:</p>

      {/* Styled button using CSS module class */}
      <button className={styles.myButton}>
        Styled Button
      </button>
    </section>
  );
}
