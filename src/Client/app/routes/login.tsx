import type { LoaderFunction } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  await auth.authenticate("oidc", request, {
    successRedirect: "/",
    failureRedirect: "/"
  });

  return {};
};
