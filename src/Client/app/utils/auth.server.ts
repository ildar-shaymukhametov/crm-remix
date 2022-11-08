import { createCookieSessionStorage } from "@remix-run/node";
import { OidcAuthenticator } from "./oidc-authenticator";
import { OidcStrategy } from "./oidc-strategy";

const sessionSecret = process.env.SESSION_SECRET;
if (!sessionSecret) {
  throw new Error("SESSION_SECRET must be set");
}

// todo: extract to session.server.ts
const storage = createCookieSessionStorage({
  cookie: {
    name: "_session",
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
      scope: "openid profile CRM.ApiAPI",
      authority: "https://localhost:5001",
      nonce: "nonce",
    },
    async ({ accessToken, refreshToken, extraParams, profile }) => {
      extraParams.access_token = accessToken;
      return { ...profile, extra: { ...extraParams } };
    }
  )
);

export const { getSession, commitSession, destroySession } = storage;