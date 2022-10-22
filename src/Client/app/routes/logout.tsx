import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const action: ActionFunction = async ({ request }) => {
  let user = await auth.isAuthenticated(request);
  console.log(user);

  return await auth.logout(request, {
    redirectTo: `https://localhost:5001/connect/endsession?id_token_hint=${user.id_token}&post_logout_redirect_uri=http://localhost:3000/authentication/logout-callback`,
  });
};

export const loader: LoaderFunction = async () => {
  return redirect("/");
};
