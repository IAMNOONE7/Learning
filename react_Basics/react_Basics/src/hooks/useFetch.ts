import { useEffect, useState } from "react";

/*
  useFetch - simple reusable API fetch hook

  Example:
    const { data, loading, error } = useFetch("https://...");
*/
export function useFetch<T>(url: string) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setLoading(true);

    fetch(url)
      .then((res) => {
        if (!res.ok) throw new Error("Error fetching data");
        return res.json();
      })
      .then((json) => setData(json))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [url]);

  return { data, loading, error };
}
