import type { Page } from "@playwright/test";
import { test as base } from "@playwright/test";
import { parse } from "cookie";
import invariant from "tiny-invariant";
import { commitSession, getSession } from "~/utils/session.server";
import type { OidcProfile } from "~/utils/oidc-strategy";
import type { NewUser } from "~/utils/user.server";
import { createUser } from "~/utils/user.server";
import { faker } from "@faker-js/faker";

type DefaultUserOptions = {
  claims?: string[];
  accessToken?: string;
  givenName?: string;
  familiyName?: string;
};

let adminAccessToken = "";

const adminUser: NewUser = {
  password: "Administrator1!",
  userName: "administrator@localhost",
  firstName: "admin",
  lastName: "admin-localhost",
  roles: ["Administrator"]
};

const defaultUser: NewUser = {
  password: "Default1!",
  userName: "default@localhost",
  firstName: "default",
  lastName: "default-localhost"
};

export const test = base.extend<{
  runAsDefaultUser: (options?: DefaultUserOptions) => Promise<User>;
  runAsAdministrator: () => Promise<OidcProfile>;
  resetDb: () => Promise<void>;
  createUser: () => Promise<User>;
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
        await page.request.post(`${process.env.API_URL}/test/resetdb`, {
          headers: {
            "X-API-Key": "TestApiKey"
          }
        });
        adminAccessToken = "";
      });
    },
    { auto: true }
  ],
  createUser: [
    async ({ baseURL }, use) => {
      invariant(baseURL, "baseURL must be set");
      use(async () => {
        const data = createRandomUser();
        const userId = await createUser(new Request("http://foobar.com"), data);

        return {
          ...data,
          id: userId
        };
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
  const user = { ...defaultUser };
  if (options.claims && options.claims?.length > 0) {
    user.claims = options.claims;
  }

  const userId = await createUser(new Request("http://foobar.com"), user);

  const profile = createOidcProfile(
    userId,
    user.firstName,
    user.lastName,
    user.userName
  );
  profile.extra.access_token =
    options.accessToken ??
    (await getAccessToken(page, user.userName, user.password));

  await login(page, baseURL, profile);

  return {
    id: userId,
    ...user
  };
}

async function runAsAdministrator(page: Page, baseURL: string) {
  const adminId = await createUser(new Request("http://foobar.com"), adminUser);
  const user = createOidcProfile(
    adminId,
    adminUser.firstName,
    adminUser.lastName,
    adminUser.userName
  );
  user.extra.access_token = await getAccessToken(
    page,
    adminUser.userName,
    adminUser.password
  );
  adminAccessToken = user.extra.access_token;

  await login(page, baseURL, user);

  return user;
}

async function login(page: Page, baseURL: string, user: OidcProfile) {
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
}

async function getAccessToken(page: Page, username: string, password: string) {
  if (username === adminUser.userName) {
    return await requestAccessToken(
      page,
      adminUser.userName,
      adminUser.password
    );
  }

  return await requestAccessToken(page, username, password);
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

type User = {
  id: string;
  userName: string;
  password: string;
  firstName?: string;
  lastName?: string;
};

function createRandomUser(): NewUser {
  return {
    password: `${faker.internet.password()}1!`,
    userName: faker.internet.email(),
    firstName: faker.person.firstName(),
    lastName: faker.person.lastName()
  };
}

function createOidcProfile(
  id: string,
  givenName = "",
  familyName = "",
  displayName = ""
): OidcProfile {
  return {
    displayName,
    id: id,
    name: {
      familyName,
      givenName,
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

export async function getAdminAccessToken(page: Page) {
  if (adminAccessToken) {
    return adminAccessToken;
  }

  await createUser(new Request("http://foobar.com"), adminUser);

  adminAccessToken = await getAccessToken(page, adminUser.userName, adminUser.password);
  return adminAccessToken;
}
