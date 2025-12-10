import React from "react";

/*
  Error boundaries must be class components.

  They catch errors in:
  - rendering
  - lifecycle methods
  - constructors of child components

  They DO NOT catch:
  - async errors (Promises)
  - event handler errors
  - server-side rendering errors
*/
type ErrorBoundaryState = {
  hasError: boolean;
  message: string;
};

export class ErrorBoundary extends React.Component<
  { children: React.ReactNode },
  ErrorBoundaryState
> {
  constructor(props: { children: React.ReactNode }) {
    super(props);

    // Initial fallback state
    this.state = {
      hasError: false,
      message: "",
    };
  }

  // React calls this method when ANY child component inside throws an error.
  static getDerivedStateFromError(error: Error) {
    return { hasError: true, message: error.message };
  }

  // Perfect place to log errors to server, analytics, etc.
  componentDidCatch(error: Error, info: React.ErrorInfo) {
    console.error("Error caught by ErrorBoundary:", error);
    console.error("Component stack:", info.componentStack);
  }

  render() {
    if (this.state.hasError) {
      return (
        <div style={{ padding: "1rem", border: "1px solid red" }}>
          <h3 style={{ color: "red" }}>Something went wrong.</h3>
          <p>{this.state.message}</p>
        </div>
      );
    }

    // No error -> render children normally
    return this.props.children;
  }
}
