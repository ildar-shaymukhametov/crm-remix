import { redirect } from "@remix-run/node";
import type { Session } from "@remix-run/server-runtime";
import { Authenticator } from "remix-auth";
import invariant from "tiny-invariant";
import { getSession, commitSession } from "~/utils/session.server";
import { auth } from "~/utils/auth.server";
import type { OidcProfile } from "./oidc-strategy";
import { getUserPermissions } from "./user.server";

export class OidcAuthenticator extends Authenticator<OidcProfile> {
  async requireUser(
    request: Request,
    { permissions }: { permissions: string[] } = { permissions: [] }
  ): Promise<OidcProfile> {
    const user = await auth.isAuthenticated(request);
    if (user) {
      if (permissions.length === 0) {
        return user;
      }

      user.permissions = await getUserPermissions(
        permissions,
        user.extra?.access_token
      );

      return user;
    }

    const session = await getSession(request.headers.get("Cookie"));
    session.set("returnUrl", request.url);
    throw redirect("/login", {
      headers: {
        "Set-Cookie": await commitSession(session)
      }
    });
  }

  logout(
    request: Request | Session,
    options: { user: OidcProfile; redirectTo: string }
  ): Promise<never> {
    invariant(
      process.env.POST_LOGOUT_REDIRECT_URI,
      "POST_LOGOUT_REDIRECT_URI is not defined"
    );
    invariant(process.env.ENDSESSION_URL, "ENDSESSION_URL is not defined");

    const query = new URLSearchParams({
      id_token_hint: options.user.extra.id_token,
      post_logout_redirect_uri: process.env.POST_LOGOUT_REDIRECT_URI
    });
    const url = new URL(`${process.env.ENDSESSION_URL}?${query}`);
    return super.logout(request, {
      redirectTo: url.toString()
    });
  }
}
