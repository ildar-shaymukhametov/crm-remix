import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  return json(user);
};

export default function Screen() {
  const user = useLoaderData();
  return (
    <pre>
      <code>{JSON.stringify(user, null, 2)}</code>
    </pre>
  );
}
