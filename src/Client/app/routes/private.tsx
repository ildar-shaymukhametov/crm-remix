import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Form, useLoaderData } from "@remix-run/react";
import type { OidcProfile } from "~/utils/oidc-strategy";

type LoaderData = { user: OidcProfile };

export const loader: LoaderFunction = async ({ request, context }) => {
  const { user } = context;

  return json<LoaderData>({ user });
};

export default function Screen() {
  const { user } = useLoaderData<LoaderData>();
  return (
    <>
      <Form method="post" action="/logout">
        <button>Log Out</button>
      </Form>

      <hr />

      <pre>
        <code>{JSON.stringify(user, null, 2)}</code>
      </pre>
    </>
  );
}
