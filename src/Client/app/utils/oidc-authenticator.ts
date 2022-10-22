import type { Session } from "@remix-run/server-runtime";
import { Authenticator } from "remix-auth";
import invariant from "tiny-invariant";
import type { OidcProfile } from "./oidc-strategy";

export class OidcAuthenticator extends Authenticator<OidcProfile> {
  isAuthenticated(request: Request | Session): Promise<OidcProfile> {
    return super.isAuthenticated(request, {
      failureRedirect: "/login",
    });
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
