import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  return await auth.logout(request, { user, redirectTo: "/" });
};

export const loader: LoaderFunction = async () => {
  return redirect("/");
};
