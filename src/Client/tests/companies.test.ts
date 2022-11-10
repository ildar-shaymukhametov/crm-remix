import { expect } from "@playwright/test";
import { test } from "./companies-test";

test.afterEach(async ({ resetDb }) => {
  await resetDb();
});

test("companies page for default user", async ({
  page,
  createCompany,
  runAsDefaultUser,
}) => {
  await runAsDefaultUser();
  let company = await createCompany();

  await page.goto("/companies");
  await expect(page).toHaveTitle(/companies/i);

  let companyName = page.getByText(company.name);
  await expect(companyName).toBeVisible();

  let companiesNotFound = page.getByText(/no companies found/i);
  await expect(companiesNotFound).not.toBeVisible();

  let button = page.getByRole("button", { name: /new company/i });
  await expect(button).not.toBeVisible();

  await page.goto("/companies/new");
  let error = page.getByText(/forbidden/i);
  await expect(error).toBeVisible();
});

test("companies page for admin user", async ({ page, runAsAdministrator }) => {
  await runAsAdministrator();
  await page.goto("/companies");

  let companiesNotFound = page.getByText(/no companies found/i);
  await expect(companiesNotFound).toBeVisible();

  let button = page.getByRole("link", { name: /new company/i });
  await expect(button).toBeVisible();

  await button.click();
  await expect(page).toHaveURL("/companies/new");

  let error = page.getByText(/forbidden/i);
  await expect(error).not.toBeVisible();
});
