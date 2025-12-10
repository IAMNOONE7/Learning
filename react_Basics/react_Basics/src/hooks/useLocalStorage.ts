import { useState, useEffect } from "react";

/*
  useLocalStorage - stores value in browser localStorage
  and syncs it across reloads.

  Example:
    const [theme, setTheme] = useLocalStorage("theme", "light");
*/
export function useLocalStorage<T>(key: string, defaultValue: T) {
  // Load initial value from localStorage
  const [value, setValue] = useState<T>(() => {
    const json = localStorage.getItem(key);
    if (json == null) return defaultValue;
    return JSON.parse(json) as T;
  });

  // Save to localStorage whenever value changes
  useEffect(() => {
    localStorage.setItem(key, JSON.stringify(value));
  }, [key, value]);

  return [value, setValue] as const;
}
