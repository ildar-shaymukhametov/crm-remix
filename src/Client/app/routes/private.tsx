import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Form, useLoaderData } from "@remix-run/react";
import type { OAuth2Profile } from "remix-auth-oauth2";

import { auth } from "~/utils/auth.server";

type LoaderData = { profile: OAuth2Profile };

export const loader: LoaderFunction = async ({ request }) => {
  const profile = await auth.isAuthenticated(request, {
    failureRedirect: "/",
  });

  return json<LoaderData>({ profile });
};

export default function Screen() {
  const { profile } = useLoaderData<LoaderData>();
  return (
    <>
      <Form method="post" action="/logout">
        <button>Log Out</button>
      </Form>

      <hr />

      <pre>
        <code>{JSON.stringify(profile, null, 2)}</code>
      </pre>
    </>
  );
}
