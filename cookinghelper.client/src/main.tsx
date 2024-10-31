import React from "react";
import ReactDOM from "react-dom/client";
import SingInPage from "./SingInPage";
import { ProtectedRoute } from "./ProtectedRoute";
import AdminPage from "./AdminPage";
import "./index.css";
import GlobalStyles from "@mui/material/GlobalStyles";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ErrorElement from "./ErrorElement";

/*
  login後 進 /admin 的 route
*/
const router = createBrowserRouter([
  SingInPage,
  {
    element: <ProtectedRoute navigationPath="/" />,
    errorElement: <ErrorElement />,
    children: [{ ...AdminPage }],
  },
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <GlobalStyles
      styles={{
        "#root": {
          width: "100%",
          height: "100%",
          margin: "0 auto",
          padding: "2rem",
        },
      }}
    />
    <RouterProvider router={router} />
  </React.StrictMode>,
);
