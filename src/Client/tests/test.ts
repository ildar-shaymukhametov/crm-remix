import { test as base } from "@playwright/test";
import { parse } from "cookie";
import { commitSession, getSession } from "~/utils/auth.server";
import type { OidcProfile } from "~/utils/oidc-strategy";

export type TestOptions = {
  login: () => Promise<OidcProfile>;
};

export const test = base.extend<TestOptions>({
  login: [
    async ({ page, baseURL }, use) => {
      use(async () => {
        const user = createUser();
        const session = await getSession();
        session.set('user', user);
        const cookieValue = await commitSession(session);
        const { _session } = parse(cookieValue)
        page.context().addCookies([
          {
            name: "_session",
            sameSite: "Lax",
            url: baseURL,
            httpOnly: true,
            secure: process.env.NODE_ENV === "production",
            value: _session,
          },
        ]);
        return user;
      });
    },
    { auto: true },
  ],
});

function createUser(): OidcProfile {
  return {
    displayName: "test@localhost",
    id: "testUser",
    name: {
      familyName: "",
      givenName: "",
      middleName: "",
    },
    emails: [],
    photos: [],
    authrizationClaims: [],
    extra: {
      expires_in: 86400,
      id_token: "",
      scope: "",
      token_type: "Bearer",
    },
    _json: {
      sub: "",
      name: "",
      given_name: "",
      family_name: "",
      middle_name: "",
      nickname: "",
      preferred_username: "",
      profile: "",
      picture: "",
      website: "",
      email: "",
      email_verified: false,
      gender: "",
      birthdate: "",
      zoneinfo: "",
      locale: "",
      phone_number: "",
      phone_number_verified: false,
      address: {
        country: "",
      },
      updated_at: "",
    },
    provider: "",
  };
}
