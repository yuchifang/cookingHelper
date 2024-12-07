import ErrorElement from "../ErrorElement";

export default {
  path: "/",
  async lazy() {
    const { SignInPage, loader } = await import(
      /*webpackChunkName: "SignInPage"*/ "./SignInPage"
    );
    return { Component: SignInPage, loader, ErrorBoundary: ErrorElement };
  },
};
