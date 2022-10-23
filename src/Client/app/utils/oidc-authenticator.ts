import { redirect } from "@remix-run/node";
import type { Session } from "@remix-run/server-runtime";
import { Authenticator } from "remix-auth";
import invariant from "tiny-invariant";
import { auth, getSession, commitSession } from "./auth.server";
import type { OidcProfile } from "./oidc-strategy";

export class OidcAuthenticator extends Authenticator<OidcProfile> {
  async requireUser(request: Request): Promise<OidcProfile> {
    const user = await auth.isAuthenticated(request);
    if (user) {
      return user;
    }

    const session = await getSession(request);
    session.set("returnUrl", request.url);
    throw redirect("/login", {
      headers: {
        "Set-Cookie": await commitSession(session)
      }
    })
  }

  logout(request: Request | Session, user: OidcProfile): Promise<never> {
    invariant(
      process.env.POST_LOGOUT_REDIRECT_URI,
      "POST_LOGOUT_REDIRECT_URI is not defined"
    );
    invariant(process.env.ENDSESSION_URL, "ENDSESSION_URL is not defined");

    var query = new URLSearchParams({
      id_token_hint: user.extra.id_token,
      post_logout_redirect_uri: process.env.POST_LOGOUT_REDIRECT_URI,
    });
    var url = new URL(`${process.env.ENDSESSION_URL}?${query}`);
    return super.logout(request, {
      redirectTo: url.toString(),
    });
  }
}
