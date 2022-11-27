import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const action: ActionFunction = async ({ request }) => {
  return await auth.logout(request, {
    redirectTo: "/login",
    rememberReturnUrl: false
  });
};

export const loader: LoaderFunction = async () => {
  return redirect("/");
};
