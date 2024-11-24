import ErrorElement from "../ErrorElement";
import { SignInPage, action, loader } from "./SingInPage";

export default {
  path: "/",
  element: <SignInPage />,
  errorElement: <ErrorElement />,
  action: action,
  loader: loader,
};
