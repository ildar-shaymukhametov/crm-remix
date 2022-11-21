import invariant from "tiny-invariant";
import { OidcAuthenticator } from "./oidc-authenticator";
import { OidcStrategy } from "./oidc-strategy";
import { sessionStorage } from "./session.server";

invariant(process.env.CLIENT_ID, "CLIENT_ID must be set");
invariant(process.env.CLIENT_SECRET, "CLIENT_SECRET must be set");
invariant(process.env.CALLBACK_URL, "CALLBACK_URL must be set");

export const auth = new OidcAuthenticator(sessionStorage);
auth.use(
  new OidcStrategy(
    {
      clientID: process.env.CLIENT_ID,
      clientSecret: process.env.CLIENT_SECRET,
      callbackURL: process.env.CALLBACK_URL,
      scope: "openid profile CRM.ApiAPI",
      authority: "https://localhost:5001",
      nonce: "nonce"
    },
    async ({ accessToken, refreshToken, extraParams, profile }) => {
      extraParams.access_token = accessToken;
      return { ...profile, extra: { ...extraParams } };
    }
  )
);
