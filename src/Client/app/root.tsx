import type { LinksFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import {
  Links,
  LiveReload,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  isRouteErrorResponse,
  useRouteError
} from "@remix-run/react";
import Navbar from "./components/navbar";
import styles from "./styles/tailwind.css";
import { auth } from "./utils/auth.server";

export const links: LinksFunction = () => {
  return [{ rel: "stylesheet", href: styles }];
};

export const meta = () => [
  {
    title: "New Remix App"
  }
];

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  return json({
    user: {
      displayName: user.displayName
    }
  });
};

export default function App() {
  return (
    <html lang="en" className="h-full">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width,initial-scale=1" />
        <Meta />
        <Links />
      </head>
      <body className="h-full">
        <Navbar />
        <main className="flex h-full" style={{ paddingTop: `${64}px` }}>
          <Outlet />
        </main>
        <ScrollRestoration />
        <Scripts />
        <LiveReload />
      </body>
    </html>
  );
}

export function ErrorBoundary() {
  const error = useRouteError();

  return (
    <html>
      <head>
        <title>Oops!</title>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width,initial-scale=1" />
        <Meta />
        <Links />
      </head>
      <body>
        {isRouteErrorResponse(error) ? (
          <h1>
            {error.status} {error.statusText}
          </h1>
        ) : (
          <h1>Unexpected error</h1>
        )}
        <Scripts />
      </body>
    </html>
  );
}
