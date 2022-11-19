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

test.describe("new company", () => {
  test("should be forbidden", async ({ page, runAsDefaultUser }) => {
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
  test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser({ claims: ["company.view"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}`);

    await expectMinimalUi(page, company);
  });

  test("should be forbidden", async ({
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
      notFound: false,
    });
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

  test("should see not found", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({ claims: ["company.view"] });
    await page.goto(`/companies/1`);

    await expectMinimalUi(page, undefined, {
      title: "minimal",
      companyFields: false,
      forbidden: false,
      notFound: true,
    });
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    title?: "minimal" | "full";
    editButton?: boolean;
    deleteButton?: boolean;
    notFound?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    company?: Company,
    {
      forbidden = false,
      companyFields = true,
      title = "full",
      editButton = false,
      deleteButton = false,
      notFound = false,
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("View company");
    } else {
      if (company) {
        await expect(page).toHaveTitle(company.name);
      }
    }

    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden,
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);
    if (company) {
      await expectCompanyFieldsToValues(page, company, companyFields);
    }
    await expect(page.getByRole("link", { name: /edit/i })).toBeVisible({
      visible: editButton,
    });
    await expect(page.getByRole("link", { name: /delete/i })).toBeVisible({
      visible: deleteButton,
    });
    await expect(page.getByText(/company not found/i)).toBeVisible({
      visible: notFound,
    });
  }

  async function expectCompanyFieldsToBeVisible(page: Page, visible: boolean) {
    const fields = [
      "name",
      "address",
      "ceo",
      "contacts",
      "email",
      "inn",
      "phone",
      "type",
    ];

    for (const field of fields) {
      await expect(page.locator(`[aria-label="${field}"]`)).toBeVisible({
        visible,
      });
    }
  }

  async function expectCompanyFieldsToValues(
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
  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/edit`);

    await expectMinimalUi(page, company, {
      forbidden: true,
      companyFields: false,
      submitButton: false,
      title: "minimal",
    });
  });

  test("should be able to edit company", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser({ claims: ["company.update", "company.view"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/edit`);

    await expectMinimalUi(page, company);

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
    await submit.click();

    await expect(page).toHaveURL(`/companies/${company.id}`);
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    submitButton?: boolean;
    title?: "minimal" | "full";
  };

  async function expectMinimalUi(
    page: Page,
    company: Company,
    {
      forbidden = false,
      companyFields = true,
      submitButton = true,
      title = "full",
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("Edit company");
    } else {
      await expect(page).toHaveTitle(company.name);
    }
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden,
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);

    const save = page.getByRole("button", { name: /save changes/i });
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

test.describe("delete company", () => {
  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/delete`);

    await expectMinimalUi(page, company, {
      cancelButton: false,
      forbidden: true,
      okButton: false,
      title: "minimal",
    });
  });

  test("should be able to delete company", async ({
    page,
    runAsDefaultUser,
    createCompany,
  }) => {
    await runAsDefaultUser({ claims: ["company.delete", "company.view"] });
    const company = await createCompany();
    await page.goto(`/companies/${company.id}/delete`);

    await expectMinimalUi(page, company);

    const deleteButton = page.getByRole("button", {
      name: /delete company/i,
    });
    await deleteButton.click();

    await expect(page).toHaveURL("/companies");
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    okButton?: boolean;
    cancelButton?: boolean;
    title?: "minimal" | "full";
  };

  async function expectMinimalUi(
    page: Page,
    company: Company,
    {
      forbidden = false,
      okButton = true,
      cancelButton = true,
      title = "full",
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("Delete company");
    } else {
      await expect(page).toHaveTitle(`${company.name} • Delete`);
    }
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden,
    });

    const deleteButtonElem = page.getByRole("button", {
      name: /delete company/i,
    });
    await expect(deleteButtonElem).toBeVisible({
      visible: okButton,
    });

    const cancelButtonElem = page.getByRole("link", { name: /cancel/i });
    await expect(cancelButtonElem).toBeVisible({
      visible: cancelButton,
    });
  }
});
