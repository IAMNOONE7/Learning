export function CrashyComponent() {
  throw new Error("Intentional crash for testing Error Boundaries.");

  // This return will never happen (we crash first)
  return <p>You will never see this.</p>;
}
