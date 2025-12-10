import { ErrorBoundary } from "./ErrorBoundary";
import { CrashyComponent } from "./CrashyComponent";

type LessonTwelveProps = {
  topic: string;
};

export function LessonTwelve({ topic }: LessonTwelveProps) {
  return (
    <section>
      <h2>Lesson 12: {topic}</h2>

      <p>
        Below, we intentionally render a component that crashes. 
        Thanks to Error Boundaries, the rest of the app continues working.
      </p>

      <ErrorBoundary>
        <CrashyComponent />
      </ErrorBoundary>

      <p style={{ marginTop: "1rem" }}>
        The app is still running! Error boundaries protect the UI.
      </p>
    </section>
  );
}
