import { useState } from "react";

export default function Response() {
  const [response, setResponse] = useState("");

  return (
    <div>
      <p>{response}</p>
    </div>
  );
}
