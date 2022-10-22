import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const action: ActionFunction = async ({ request }) => {
  let user = await auth.isAuthenticated(request);
  return await auth.logout(request, user);
};

export const loader: LoaderFunction = async () => {
  return redirect("/");
};
