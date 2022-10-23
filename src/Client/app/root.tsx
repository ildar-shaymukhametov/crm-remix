import type {
  LinksFunction,
  LoaderFunction,
  MetaFunction} from "@remix-run/node";
import {
  json
} from "@remix-run/node";
import {
  Links,
  LiveReload,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
} from "@remix-run/react";
import Header from "./components/header";
import Sidebar from "./components/sidebar";
import styles from "./styles/tailwind.css";
import { auth } from "./utils/auth.server";

export const links: LinksFunction = () => {
  return [{ rel: "stylesheet", href: styles }];
};

export const meta: MetaFunction = () => ({
  charset: "utf-8",
  title: "New Remix App",
  viewport: "width=device-width,initial-scale=1",
});

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  console.log(user);
  
  return json({
    user: user,
  });
};

export default function App() {
  return (
    <html lang="en" className="h-full">
      <head>
        <Meta />
        <Links />
      </head>
      <body className="h-full">
        <Header />
        <main className="flex h-full">
          <Sidebar />
          <div className="flex-1">
            <Outlet />
          </div>
        </main>
        <ScrollRestoration />
        <Scripts />
        <LiveReload />
      </body>
    </html>
  );
}
