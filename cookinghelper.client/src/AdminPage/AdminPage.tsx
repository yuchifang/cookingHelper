import { useLocation } from "react-router-dom";

//? 阻止 直接進 admin??
export function AdminPage() {
  const location = useLocation();
  console.log("location", location);
  return <div>AdminPage</div>;
}
