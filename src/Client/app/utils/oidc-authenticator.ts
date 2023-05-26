import { redirect } from "@remix-run/node";
import { Authenticator } from "remix-auth";
import invariant from "tiny-invariant";
import {
  destroySession,
  getSession,
  returnUrlSession
} from "~/utils/session.server";
import { auth } from "~/utils/auth.server";
import type { OidcProfile } from "./oidc-strategy";
import type { ResourcePermissions } from "./user.server";
import { getUserPermissions } from "./user.server";
import { handle401Response } from "./utils";

export class OidcAuthenticator extends Authenticator<OidcProfile> {
  async requireUser(
    request: Request,
    resource?: ResourcePermissions
  ): Promise<OidcProfile> {
    const user = await auth.isAuthenticated(request);
    if (user) {
      if (!resource || resource.permissions?.length === 0) {
        return user;
      }

      user.permissions = await getUserPermissions(
        request,
        resource,
        user.extra?.access_token
      );

      return user;
    }

    return await handle401Response(request);
  }

  async logout(
    request: Request,
    {
      redirectTo,
      rememberReturnUrl
    }: { redirectTo: string; rememberReturnUrl: boolean }
  ): Promise<never> {
    invariant(
      process.env.POST_LOGOUT_REDIRECT_URI,
      "POST_LOGOUT_REDIRECT_URI is not defined"
    );
    invariant(process.env.ENDSESSION_URL, "ENDSESSION_URL is not defined");

    const user = await this.isAuthenticated(request);
    if (user) {
      const params = new URLSearchParams({
        id_token_hint: user.extra.id_token,
        post_logout_redirect_uri: process.env.POST_LOGOUT_REDIRECT_URI
      });
      redirectTo = new URL(
        `${process.env.ENDSESSION_URL}?${params}`
      ).toString();
    }

    const session = await getSession(request.headers.get("Cookie"));
    const headers: HeadersInit = [
      ["Set-Cookie", await destroySession(session)]
    ];

    if (rememberReturnUrl) {
      const returnUrl = new URL(request.url).pathname;
      const returnUrlStorage = await returnUrlSession.getSession(
        request.headers.get("Cookie")
      );
      returnUrlStorage.set("returnUrl", returnUrl);
      headers.push([
        "Set-Cookie",
        await returnUrlSession.commitSession(returnUrlStorage)
      ]);
      console.log(`🟢 Logout: return url: ${returnUrl}`);
    }

    console.log(`🟢 Logout: redirect to: ${redirectTo}`);

    throw redirect(redirectTo, {
      headers
    });
  }
}
