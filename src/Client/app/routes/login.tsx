import { Form } from "@remix-run/react";
import type { ActionFunction, LoaderFunction } from "@remix-run/node";

import { auth } from "~/utils/auth.server";

// export const loader: LoaderFunction = async () => redirect("/");

export const action: ActionFunction = async ({ request }) => {
  return auth.authenticate("oidc", request);
};

export default function Screen() {
  return (
    <Form method="post">
      <button>Sign In with Auth0</button>
    </Form>
  );
}
