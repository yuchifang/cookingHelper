import ErrorElement from "../ErrorElement";
import { SignInPage, action } from "./SingInPage";

export default {
  path: "/",
  element: <SignInPage />,
  errorElement: <ErrorElement />,
  action: action,
};
