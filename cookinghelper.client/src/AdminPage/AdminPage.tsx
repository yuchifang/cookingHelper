import { useLocation } from "react-router-dom";

export function AdminPage() {
  const location = useLocation();
  console.log("location", location);
  return <div>AdminPage</div>;
}
