import type { Page } from "@playwright/test";
import { test as base } from "@playwright/test";
import { parse } from "cookie";
import invariant from "tiny-invariant";
import { commitSession, getSession } from "~/utils/session.server";
import type { OidcProfile } from "~/utils/oidc-strategy";
import { updateAuthorizationClaims } from "~/utils/account.server";

let defaultUserAccessToken = "";
let adminAccessToken = "";

type DefaultUserOptions = {
  claims?: string[];
  accessToken?: string;
};

export const test = base.extend<{
  runAsDefaultUser: (options?: DefaultUserOptions) => Promise<OidcProfile>;
  runAsAdministrator: () => Promise<OidcProfile>;
  resetDb: () => Promise<void>;
}>({
  runAsDefaultUser: [
    async ({ page, baseURL }, use) => {
      invariant(baseURL, "baseURL must be set");
      use((options: DefaultUserOptions = { claims: [] }) =>
        runAsDefaultUser(page, baseURL, options)
      );
    },
    { auto: true }
  ],
  runAsAdministrator: [
    async ({ page, baseURL }, use) => {
      invariant(baseURL, "baseURL must be set");
      use(() => runAsAdministrator(page, baseURL));
    },
    { auto: true }
  ],
  resetDb: [
    async ({ page }, use) => {
      use(async () => {
        const token = await getAdminAccessToken(page);
        await page.request.post(`${process.env.API_URL}/test/resetdb`, {
          headers: {
            Authorization: `Bearer ${token}`
          }
        });
      });
    },
    { auto: true }
  ]
});

async function runAsDefaultUser(
  page: Page,
  baseURL: string,
  options: DefaultUserOptions
) {
  const user = await login(
    page,
    baseURL,
    "tester@localhost",
    "Tester1!",
    "f3ad4d5d-1ebe-4c22-8570-ca237fd1c7c4",
    options.accessToken,
  );
  if (options.claims && options.claims.length > 0) {
    await updateAuthorizationClaims(
      new Request("http://foobar.com"),
      { claims: options.claims },
      user.extra?.access_token
    );
  }
  return user;
}

function runAsAdministrator(page: Page, baseURL: string) {
  return login(
    page,
    baseURL,
    "administrator@localhost",
    "Administrator1!",
    "0ad62604-08dc-400b-b1f8-4cdb7f2e7674"
  );
}

async function login(
  page: Page,
  baseURL: string,
  username: string,
  password: string,
  id: string,
  accessToken?: string,
) {
  const user = createUser(id);
  user.extra.access_token =
    accessToken ?? (await getAccessToken(page, username, password));

  const session = await getSession();
  session.set("user", user);
  const cookieValue = await commitSession(session);
  const { _session } = parse(cookieValue);
  page.context().addCookies([
    {
      name: "_session",
      sameSite: "Lax",
      url: baseURL,
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      value: _session
    }
  ]);
  return user;
}

async function getAccessToken(page: Page, username: string, password: string) {
  if (username === "administrator@localhost") {
    return await getAdminAccessToken(page);
  }

  return await getDefaultUserAccessToken(page, username, password);
}

export async function getDefaultUserAccessToken(
  page: Page,
  username = "tester@localhost",
  password = "Tester1!"
) {
  if (defaultUserAccessToken) {
    return defaultUserAccessToken;
  }

  defaultUserAccessToken = await requestAccessToken(page, username, password);
  return defaultUserAccessToken;
}

export async function getAdminAccessToken(page: Page) {
  if (adminAccessToken) {
    return adminAccessToken;
  }

  adminAccessToken = await requestAccessToken(
    page,
    "administrator@localhost",
    "Administrator1!"
  );
  return adminAccessToken;
}

async function requestAccessToken(
  page: Page,
  username: string,
  password: string
) {
  const response = await page.request.post(
    `${process.env.AUTHORITY}/connect/token`,
    {
      form: {
        grant_type: "password",
        scope: "openid profile CRM.ApiAPI",
        username,
        password
      },
      headers: {
        Authorization: `Basic ${Buffer.from(
          `${process.env.CLIENT_ID}:${process.env.CLIENT_SECRET}`
        ).toString("base64")}`
      }
    }
  );

  const { access_token } = await response.json();

  return access_token as string;
}

function createUser(id: string): OidcProfile {
  return {
    displayName: "test@localhost",
    id: id,
    name: {
      familyName: "",
      givenName: "",
      middleName: ""
    },
    emails: [],
    photos: [],
    permissions: [],
    extra: {
      expires_in: 86400,
      id_token: "",
      scope: "",
      token_type: "Bearer",
      access_token: ""
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
        country: ""
      },
      updated_at: ""
    },
    provider: ""
  };
}
