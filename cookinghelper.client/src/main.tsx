import React from "react";
import ReactDOM from "react-dom/client";
import SingInPage from "./SingInPage";

import AdminPage from "./AdminPage";
import "./index.css";
import GlobalStyles from "@mui/material/GlobalStyles";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import ResetPassword from "./ResetPassword";

const router = createBrowserRouter([SingInPage, AdminPage, ResetPassword]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <GlobalStyles
      styles={{
        "#root": {
          margin: "0 auto",
          padding: "3rem",
        },
      }}
    />
    <RouterProvider router={router} />
  </React.StrictMode>,
);
