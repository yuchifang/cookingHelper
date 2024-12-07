import ErrorElement from "../ErrorElement";

export default {
  path: "/admin",
  async lazy() {
    const { AdminPage, loader } = await import(
      /*webpackChunkName: "AdminPage"*/ "./AdminPage"
    );
    return { Component: AdminPage, ErrorBoundary: ErrorElement, loader };
  },
};
