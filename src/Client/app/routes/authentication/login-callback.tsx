import type { LoaderFunction } from "@remix-run/node";
import { auth } from "~/utils/auth.server";
import { returnUrlSession } from "~/utils/session.server";

export const loader: LoaderFunction = async ({ request }) => {
  const session = await returnUrlSession.getSession(
    request.headers.get("Cookie")
  );
  const returnUrl = session.get("returnUrl") ?? "/";

  console.log(`🟢 Login-callback: return url: ${returnUrl}`);

  return await auth.authenticate("oidc", request, {
    successRedirect: returnUrl,
    failureRedirect: "/"
  });
};
