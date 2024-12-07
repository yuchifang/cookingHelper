import ErrorElement from "../ErrorElement";

export default {
  path: "/reset-password",
  async lazy() {
    const { ResetPassword } = await import(
      /*webpackChunkName: "ResetPassword"*/ "./ResetPassword"
    );
    return { Component: ResetPassword, ErrorBoundary: ErrorElement };
  },
};
