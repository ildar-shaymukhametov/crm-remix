import type { LoaderFunction } from "@remix-run/node";
import { useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.requireUser(request);
};

export default function Screen() {
  const user = useLoaderData();
  return (
    <pre className="whitespace-pre-wrap break-all">
      {JSON.stringify(user, null, 2)}
    </pre>
  );
}

export function PrivateNavbarButtons() {
  return <button>Private</button>;
}
