import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { test } from "./test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("should login", async ({ page }) => {
  await page.goto("/");
  await page.waitForURL(`${process.env.AUTHORITY}/Identity/Account/Login**`);
  await page.getByLabel("Email").fill("tester@localhost");
  await page.getByLabel("Password").fill("Tester1!");
  await page.getByRole("button", { name: /log in/i }).click();
  await expect(page).toHaveURL("/");
  await expect(
    page.getByRole("link", { name: "tester@localhost" })
  ).toBeVisible();

  await page.goto("/login");
  await expect(page).toHaveURL("/");

  await page.context().clearCookies();
  await page.reload();
  await page.waitForURL(`${process.env.AUTHORITY}/Identity/Account/Login**`);
});

test("should return to correct url", async ({ page }) => {
  await page.goto(routes.companies.index);
  await page.waitForURL(`${process.env.AUTHORITY}/Identity/Account/Login**`);
  await page.getByLabel("Email").fill("tester@localhost");
  await page.getByLabel("Password").fill("Tester1!");
  await page.getByRole("button", { name: /log in/i }).click();
  await page.waitForURL(routes.companies.index);
});

test("should logout if bad access token", async ({
  page,
  runAsDefaultUser
}) => {
  await runAsDefaultUser({ accessToken: "bad_token" });
  await page.goto(routes.companies.index);
  await expect(
    page.getByText(/you have successfully logged out/i)
  ).toBeVisible();
});

test("should logout if clicked logout button", async ({
  page,
  runAsDefaultUser
}) => {
  await runAsDefaultUser();
  await page.goto("/");
  await page.getByRole("button", { name: /log out/i }).click();
  await expect(
    page.getByText(/you have successfully logged out/i)
  ).toBeVisible();
});
