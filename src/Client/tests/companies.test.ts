import { expect } from "@playwright/test";
import { test } from "./companies-test";

test.afterEach(async ({ resetDb }) => {
  await resetDb();
});

test("title", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser();
  await page.goto("/companies");
  await expect(page).toHaveTitle(/companies/i);
});

test("displays companies", async ({ page, createCompany, runAsDefaultUser }) => {
  await runAsDefaultUser();
  let company = await createCompany();

  await page.goto("/companies");

  let elem = page.getByText(company.name);
  await expect(elem).toBeVisible();
});

test("user can go to new company page", async ({ page, runAsAdministrator }) => {
  await runAsAdministrator();
  await page.goto("/companies");

  let button = page.getByRole("link", { name: /new company/i });
  await expect(button).toBeVisible();

  await button.click();
  await expect(page).toHaveURL("/companies/new");

  let error = page.getByText(/forbidden/i);
  await expect(error).not.toBeVisible();
});

test("user cannot go to new company page", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser();
  await page.goto("/companies");

  let button = page.getByRole("button", { name: /new company/i });
  await expect(button).not.toBeVisible();

  await page.goto("/companies/new");
  let error = page.getByText(/forbidden/i);
  await expect(error).toBeVisible();
});
