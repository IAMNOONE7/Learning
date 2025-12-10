import { useState } from "react";

/*
  useToggle - simple boolean state hook
  Example:
    const [isOpen, toggleIsOpen] = useToggle();
*/
export function useToggle(initialValue: boolean = false) {
  const [value, setValue] = useState(initialValue);

  function toggle() {
    setValue((prev) => !prev); // switches true <-> false
  }

  return [value, toggle] as const; 
  // "as const" keeps the tuple type stable
}
