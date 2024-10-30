import React from "react";
import ReactDOM from "react-dom/client";
import SingInPage from "./SingInPage";
import AdminPage from "./AdminPage";
import "./index.css";
import GlobalStyles from "@mui/material/GlobalStyles";
import { createBrowserRouter, RouterProvider } from "react-router-dom";

/*
  login後 進 /admin 的 route
*/
const router = createBrowserRouter([SingInPage, AdminPage]);

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <GlobalStyles
      styles={{
        "#root": {
          maxWidth: "100%",
          margin: "0 auto",
          padding: "2rem",
          TextAlign: "center",
        },
      }}
    />
    <RouterProvider router={router} />
  </React.StrictMode>,
);
