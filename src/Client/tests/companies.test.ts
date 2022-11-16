import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/routes/__layout/companies/index";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe.only("view companies", () => {
  test("minimum permissions", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser();
    await createCompany();
    await page.goto("/companies");

    await expectMinimumUi(page);
  });

  test("should see new company button", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({ claims: ["company.create"] });
    await page.goto("/companies");

    await expectMinimumUi(page, { newCompanyButton: false });

    const link = page.getByRole("link", { name: /new company/i });
    await expect(link).toBeVisible();

    await link.click();
    await expect(page).toHaveURL(/companies\/new/i);
  });

  type IncludeOptions = {
    newCompanyButton?: boolean;
    noCompaniesFound?: boolean;
  };
  async function expectMinimumUi(
    page: Page,
    { newCompanyButton, noCompaniesFound }: IncludeOptions = {
      newCompanyButton: true,
      noCompaniesFound: true,
    }
  ) {
    await expect(page).toHaveTitle(/companies/i);

    if (newCompanyButton) {
      const link = page.getByRole("link", { name: /new company/i });
      await expect(link).not.toBeVisible();
    }
    if (noCompaniesFound) {
      const companiesNotFound = page.getByText(/no companies found/i);
      await expect(companiesNotFound).toBeVisible();
    }
  }
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

    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });
});

test.describe("view company", () => {
  test("default user", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    const forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).toBeVisible();
  });

  test("admin", async ({ page, runAsAdministrator, createCompany }) => {
    await runAsAdministrator();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    const deleteButton = page.getByRole("link", { name: /delete/i });
    await expect(deleteButton).toBeVisible();

    const editButton = page.getByRole("link", { name: /edit/i });
    await expect(editButton).toBeVisible();

    await expectCompanyFieldsToBeVisible(page, company);
  });

  async function expectCompanyFieldsToBeVisible(page: Page, company: Company) {
    await expect(page.getByText(company.name)).toBeVisible();
    await expect(page.getByText(company.address)).toBeVisible();
    await expect(page.getByText(company.ceo)).toBeVisible();
    await expect(page.getByText(company.contacts)).toBeVisible();
    await expect(page.getByText(company.email)).toBeVisible();
    await expect(page.getByText(company.inn)).toBeVisible();
    await expect(page.getByText(company.phone)).toBeVisible();
    await expect(page.getByText(company.type)).toBeVisible();
  }
});

test.describe("edit company", () => {
  test("default user", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/edit`);

    const forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).toBeVisible();
  });

  test("admin", async ({ page, runAsAdministrator, createCompany }) => {
    await runAsAdministrator();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/edit`);

    const forbidden = page.getByText(/forbidden/i);
    await expect(forbidden).not.toBeVisible();

    const name = page.getByLabel(/name/i);
    const address = page.getByLabel(/address/i);
    const ceo = page.getByLabel(/ceo/i);
    const contacts = page.getByLabel(/contacts/i);
    const email = page.getByLabel(/email/i);
    const inn = page.getByLabel(/inn/i);
    const phone = page.getByLabel(/phone/i);
    const type = page.getByLabel(/type/i);

    await expect(name).toHaveValue(company.name);
    await expect(address).toHaveValue(company.address);
    await expect(ceo).toHaveValue(company.ceo);
    await expect(contacts).toHaveValue(company.contacts);
    await expect(email).toHaveValue(company.email);
    await expect(inn).toHaveValue(company.inn);
    await expect(phone).toHaveValue(company.phone);
    const selectedOption = type.getByRole("option", { selected: true });
    await expect(selectedOption).toHaveText(company.type);

    const newCompany = buildCompany();
    await name.fill(newCompany.name);
    await address.fill(newCompany.address);
    await ceo.fill(newCompany.ceo);
    await contacts.fill(newCompany.contacts);
    await email.fill(newCompany.email);
    await inn.fill(newCompany.inn);
    await phone.fill(newCompany.phone);
    await type.selectOption(newCompany.type);

    const submit = page.getByRole("button", { name: /save changes/i });
    await expect(submit).toBeVisible();
    await submit.click();

    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });
});
