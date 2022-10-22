import type { Session } from "@remix-run/server-runtime";
import { Authenticator } from "remix-auth";

export class OidcAuthenticator<User = unknown> extends Authenticator<User> {
  isAuthenticated(request: Request | Session): Promise<User> {
    return super.isAuthenticated(request, {
      failureRedirect: "/login",
    });
  }
}
