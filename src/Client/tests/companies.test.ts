import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/routes/__layout/companies/index";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("view companies", () => {
  test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser();
    await createCompany();
    await page.goto("/companies");

    await expectMinimalUi(page);
  });

  test("should be able to click new company button", async ({
    page,
    runAsDefaultUser,
  }) => {
    await runAsDefaultUser({ claims: ["company.create"] });
    await page.goto("/companies");

    await expectMinimalUi(page, { newCompanyButton: true });

    const link = page.getByRole("link", { name: /new company/i });
    await link.click();
    await expect(page).toHaveURL(/companies\/new/i);
  });

  type VisibilityOptions = {
    newCompanyButton?: boolean;
    noCompaniesFound?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    {
      newCompanyButton = false,
      noCompaniesFound = true,
    }: VisibilityOptions = {}
  ) {
    await expect(page).toHaveTitle(/companies/i);

    const link = page.getByRole("link", { name: /new company/i });
    await expect(link).toBeVisible({ visible: newCompanyButton });

    const companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).toBeVisible({ visible: noCompaniesFound });
  }
});

test.describe.only("new company", () => {
  test("should see forbidden", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser();
    await page.goto("/companies/new");

    await expectMinimalUi(page, {
      forbidden: true,
      companyFields: false,
      submitButton: false,
    });
  });

  test("should be able to create new company", async ({
    page,
    runAsDefaultUser,
  }) => {
    await runAsDefaultUser({ claims: ["company.create"] });
    await page.goto("/companies/new");

    await expectMinimalUi(page);

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

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    submitButton?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    {
      forbidden = false,
      companyFields = true,
      submitButton = true,
    }: VisibilityOptions = {}
  ) {
    await expect(page).toHaveTitle("New company");
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden,
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);

    const save = page.getByRole("button", { name: /create new company/i });
    await expect(save).toBeVisible({
      visible: submitButton,
    });
  }

  async function expectCompanyFieldsToBeVisible(page: Page, visible: boolean) {
    const fields = [
      /name/i,
      /address/i,
      /ceo/i,
      /contacts/i,
      /email/i,
      /inn/i,
      /phone/i,
      /type/i,
    ];
    for (const field of fields) {
      await expect(page.getByLabel(field)).toBeVisible({ visible });
    }
  }
});

test.describe("view company", () => {
  test("should see forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    await expectMinimalUi(page, company, {
      title: "minimal",
      companyFields: false,
      forbidden: true,
    });
  });

  test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser({ claims: ["company.view"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    await expectMinimalUi(page, company);
  });

  test("should be able to click edit company button", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser({ claims: ["company.view", "company.update"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    await expectMinimalUi(page, company, { editButton: true });

    const button = page.getByRole("link", { name: /edit/i });
    await button.click();
    await expect(page).toHaveURL(`companies/${company.id}/edit`);
  });

  test("should be able to click delete company button", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser({ claims: ["company.view", "company.delete"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    await expectMinimalUi(page, company, { deleteButton: true });

    const button = page.getByRole("link", { name: /delete/i });
    await button.click();
    await expect(page).toHaveURL(`companies/${company.id}/delete`);
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    title?: "minimal" | "full";
    editButton?: boolean;
    deleteButton?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    company: Company,
    {
      forbidden = false,
      companyFields = true,
      title = "full",
      editButton = false,
      deleteButton = false,
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("View company");
    } else {
      await expect(page).toHaveTitle(company.name);
    }

    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden,
    });
    await expectCompanyFieldsToBeVisible(page, company, companyFields);
    await expect(page.getByRole("link", { name: /edit/i })).toBeVisible({
      visible: editButton,
    });
    await expect(page.getByRole("link", { name: /delete/i })).toBeVisible({
      visible: deleteButton,
    });
  }

  async function expectCompanyFieldsToBeVisible(
    page: Page,
    company: Company,
    visible = true
  ) {
    const fields = [
      company.name,
      company.address,
      company.ceo,
      company.contacts,
      company.email,
      company.inn,
      company.phone,
      company.type,
    ];

    for (const field of fields) {
      await expect(page.getByText(field)).toBeVisible({ visible });
    }
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
