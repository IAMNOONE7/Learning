import { createContext, useContext, useState } from "react";

/*
  1. Define the shape of the data inside the context
     (TypeScript interface)
*/
interface ThemeContextType {
  theme: "light" | "dark";
  toggleTheme: () => void;
}

/*
  2. Create the context.

  We pass "undefined" as default because the provider will fill it later.
  React Context ALWAYS requires a provider.
*/
const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

/*
  3. Provider component:
     Wrap entire app with <ThemeProvider> ... </ThemeProvider>
     so all children can use the theme context.
*/
export function ThemeProvider({ children }: { children: React.ReactNode }) {
  // Local state inside context
  const [theme, setTheme] = useState<"light" | "dark">("light");

  // Function to toggle theme
  function toggleTheme() {
    setTheme(theme === "light" ? "dark" : "light");
  }

  return (
    // Provide values to all children
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

/*
  4. Custom hook to access the context easily.
     Avoids writing useContext(ThemeContext) everywhere.
*/
export function useTheme() {
  const context = useContext(ThemeContext);

  if (!context) {
    throw new Error("useTheme must be used inside <ThemeProvider>");
  }

  return context;
}
