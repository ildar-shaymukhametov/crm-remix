import { expect } from "@playwright/test";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
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

    const link = page.getByRole("link", { name: /new company/i });
    await expect(link).not.toBeVisible();
  });

  test("does not see companies", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser();
    await page.goto("/companies");

    const companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).toBeVisible();
  });

  test("sees companies", async ({ page, createCompany, runAsDefaultUser }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto("/companies");

    const companyName = page.getByText(company.name);
    await expect(companyName).toBeVisible();

    const companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).not.toBeVisible();
  });

  test("navigates to new company page", async ({
    page,
    baseURL,
    runAsAdministrator,
  }) => {
    await runAsAdministrator();
    await page.goto("/companies");

    const link = page.getByRole("link", { name: /new company/i });
    await expect(link).toBeVisible();

    await link.click();
    expect(page.url()).toBe(`${baseURL}/companies/new`);
  });
});

test.describe("new company", () => {
  test("page should be forbidden", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser();
    await page.goto("/companies/new");

    const forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).toBeVisible();
  });

  test("creates new company", async ({ page, runAsAdministrator }) => {
    await runAsAdministrator();
    await page.goto("/companies/new");

    const forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).not.toBeVisible();

    const company = buildCompany();
    await page.getByLabel(/name/i).fill(company.name);
    await page.getByLabel(/address/i).fill(company.address);
    await page.getByLabel(/ceo/i).fill(company.ceo);
    await page.getByLabel(/contacts/i).fill(company.contacts);
    await page.getByLabel(/email/i).fill(company.email);
    await page.getByLabel(/inn/i).fill(company.inn);
    await page.getByLabel(/phone/i).fill(company.phone);
    const type = page.getByLabel(/type/i);
    await type.selectOption({ index: 1 });
    company.type =
      (await type.getByRole("option", { selected: true }).textContent()) ?? "";

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();

    expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });
});

test.describe("view company", () => {
  test("can view company", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    const deleteButton = page.getByRole("link", { name: /delete/i });
    await expect(deleteButton).not.toBeVisible();

    await expect(page.getByText(company.name)).toBeVisible();
    await expect(page.getByText(company.address)).toBeVisible();
    await expect(page.getByText(company.ceo)).toBeVisible();
    await expect(page.getByText(company.contacts)).toBeVisible();
    await expect(page.getByText(company.email)).toBeVisible();
    await expect(page.getByText(company.inn)).toBeVisible();
    await expect(page.getByText(company.phone)).toBeVisible();
    await expect(page.getByText(company.type)).toBeVisible();
  });

  test.only("delete button visible", async ({
    page,
    runAsAdministrator,
    createCompany,
  }) => {
    await runAsAdministrator();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    const deleteButton = page.getByRole("link", { name: /delete/i });
    await expect(deleteButton).toBeVisible();
  });
});
