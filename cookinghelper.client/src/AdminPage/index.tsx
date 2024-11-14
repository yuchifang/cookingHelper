import ErrorElement from "../ErrorElement";
import { AdminPage } from "./AdminPage";

export default {
  path: "/admin",
  element: <AdminPage />,
  errorElement: <ErrorElement />,
};
