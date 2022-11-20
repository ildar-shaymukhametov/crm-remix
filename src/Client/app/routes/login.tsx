import type { LoaderFunction } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.authenticate("oidc", request, {
    successRedirect: "/",
    failureRedirect: "/"
  });
};
