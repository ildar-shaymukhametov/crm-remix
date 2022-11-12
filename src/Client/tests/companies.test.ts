import { expect } from "@playwright/test";
import { test } from "./companies-test";

test.afterEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("companies", () => {
  test("should see title, should not see new company button", async ({
    page,
    runAsDefaultUser,
  }) => {
    await runAsDefaultUser();
    await page.goto("/companies");
    await expect(page).toHaveTitle(/companies/i);

    let link = page.getByRole("link", { name: /new company/i });
    await expect(link).not.toBeVisible();
  });

  test("does not see companies", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser();
    await page.goto("/companies");

    let companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).toBeVisible();
  });

  test("sees companies", async ({ page, createCompany, runAsDefaultUser }) => {
    await runAsDefaultUser();
    let company = await createCompany();
    await page.goto("/companies");

    let companyName = page.getByText(company.name);
    await expect(companyName).toBeVisible();

    let companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).not.toBeVisible();
  });

  test("navigates to new company page", async ({
    page,
    runAsAdministrator,
  }) => {
    await runAsAdministrator();
    await page.goto("/companies");

    let link = page.getByRole("link", { name: /new company/i });
    await expect(link).toBeVisible();

    await link.click();
    expect(page.url()).toMatch(/companies\/new/i);

    let forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).not.toBeVisible();
  });
});
