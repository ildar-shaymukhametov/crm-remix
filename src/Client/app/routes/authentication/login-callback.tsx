import type { LoaderFunction } from "@remix-run/node";

import { auth, getSession } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  const session = await getSession(request);
  return await auth.authenticate("oidc", request, {
    successRedirect: session?.get("returnUrl") ?? "/",
    failureRedirect: "/",
  });
};
