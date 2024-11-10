import React from "react";
export interface Log {
  id: number;
  logTime: number;
  userId: string;
}

export default function AnalyzeBlock({ loader }: { loader: Log[] }) {
  console.log(loader);
  return <div>AnalyzeBlock</div>;
}
