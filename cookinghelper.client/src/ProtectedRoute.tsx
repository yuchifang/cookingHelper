import { Navigate, Outlet, useLocation } from "react-router-dom";

export function ProtectedRoute({ navigationPath }: { navigationPath: string }) {
  const location = useLocation();
  return location.state ? <Outlet /> : <Navigate to={navigationPath} replace />;
}
