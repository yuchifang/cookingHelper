import ErrorElement from "../ErrorElement";
import { AdminPage, loader } from "./AdminPage";

export default {
  path: "/admin",
  element: <AdminPage />,
  errorElement: <ErrorElement />,
  loader: loader,
};
