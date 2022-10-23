import type { Session } from "@remix-run/node";
import { createCookieSessionStorage } from "@remix-run/node";
import { OidcAuthenticator } from "./oidc-authenticator";
import { OidcStrategy } from "./oidc-strategy";

const sessionSecret = process.env.SESSION_SECRET;
if (!sessionSecret) {
  throw new Error("SESSION_SECRET must be set");
}

const storage = createCookieSessionStorage({
  cookie: {
    name: "crm.session",
    secure: process.env.NODE_ENV === "production",
    secrets: [sessionSecret],
    sameSite: "lax",
    path: "/",
    maxAge: 60 * 60 * 24 * 30,
    httpOnly: true,
  },
});

export const auth = new OidcAuthenticator(storage);
auth.use(
  new OidcStrategy(
    {
      clientID: process.env.CLIENT_ID!,
      clientSecret: process.env.CLIENT_SECRET!,
      callbackURL: process.env.CALLBACK_URL!,
      scope: "openid profile",
      authority: "https://localhost:5001",
      nonce: "nonce",
    },
    async ({ accessToken, refreshToken, extraParams, profile }) => {
      return { ...profile, extra: extraParams };
    }
  )
);

export function getSession(request: Request) {
  return storage.getSession(request.headers.get("Cookie"));
}

export function commitSession(session: Session) {
  return storage.commitSession(session);
}