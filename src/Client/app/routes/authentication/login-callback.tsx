import type { LoaderFunction } from "@remix-run/node";
import { getSession } from "~/utils/session.server";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  const session = await getSession(request.headers.get("Cookie"));
  return await auth.authenticate("oidc", request, {
    successRedirect: session?.get("returnUrl") ?? "/",
    failureRedirect: "/"
  });
};
