import { useRouteError, isRouteErrorResponse } from "react-router-dom";

export default function ErrorPage() {
  // test test
  const error: any = useRouteError();
  console.error(error);
  console.log("someError", isRouteErrorResponse(error));
  return (
    <div id="error-page">
      <h1>Oops!</h1>
      <p>Sorry, an unexpected error has occurred.</p>
      <p>
        <i>
          {(error as { statusText?: string }).statusText ||
            (error as Error).message}
        </i>
      </p>
    </div>
  );
}
