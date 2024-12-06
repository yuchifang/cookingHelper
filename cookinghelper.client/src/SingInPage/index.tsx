import ErrorElement from "../ErrorElement";
import { SignInPage, loader } from "./SingInPage";

export default {
  path: "/",
  element: <SignInPage />,
  errorElement: <ErrorElement />,

  loader: loader,
};
