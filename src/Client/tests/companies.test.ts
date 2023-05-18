import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("view companies", () => {
  test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
    await runAsDefaultUser();
    await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { noCompaniesFound: true });
  });

  test("should be able to click new company button", async ({
    page,
    runAsDefaultUser
  }) => {
    await runAsDefaultUser({ claims: [claims.company.create] });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, {
      newCompanyButton: true,
      noCompaniesFound: true
    });

    const link = page.getByRole("link", { name: /new company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.new);
  });

  test("should be able to click edit company button", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.update]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { editCompanyButton: true });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(company.id));
  });

  test("should be able to click delete company button", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { deleteCompanyButton: true });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(company.id));
  });

  type VisibilityOptions = {
    newCompanyButton?: boolean;
    noCompaniesFound?: boolean;
    editCompanyButton?: boolean;
    deleteCompanyButton?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    {
      newCompanyButton = false,
      noCompaniesFound = false,
      editCompanyButton = false,
      deleteCompanyButton = false
    }: VisibilityOptions = {}
  ) {
    await expect(page).toHaveTitle(/companies/i);

    const newCompany = page.getByRole("link", { name: /new company/i });
    await expect(newCompany).toBeVisible({ visible: newCompanyButton });

    const companiesNotFound = page.getByText(/no companies found/i);
    await expect(companiesNotFound).toBeVisible({ visible: noCompaniesFound });

    const editCompany = page.getByRole("link", { name: /edit company/i });
    await expect(editCompany).toBeVisible({ visible: editCompanyButton });

    const deleteCompany = page.getByRole("link", { name: /delete company/i });
    await expect(deleteCompany).toBeVisible({ visible: deleteCompanyButton });
  }
});

test.describe("new company", () => {
  test("should be forbidden", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser();
    await page.goto(routes.companies.new);

    await expectMinimalUi(page, {
      forbidden: true,
      companyFields: false,
      submitButton: false
    });
  });

  test("should be able to create new company", async ({
    page,
    runAsDefaultUser
  }) => {
    await runAsDefaultUser({ claims: [claims.company.create] });
    await page.goto(routes.companies.new);
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

    const manager = page.getByLabel(/manager/i);
    expect(manager).toHaveText("");

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();
    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });

  test("should be able to set manager to self", async ({
    page,
    runAsDefaultUser
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.create, claims.company.new.setManagerToSelf]
    });
    await page.goto(routes.companies.new);
    await expectMinimalUi(page);

    const manager = page.getByLabel(/manager/i);
    expect(
      await manager
        .getByRole("option", {
          name: `${user.name.givenName} ${user.name.familyName}`
        })
        .textContent()
    ).toBe(`${user.name.givenName} ${user.name.familyName}`);
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
      submitButton = true
    }: VisibilityOptions = {}
  ) {
    await expect(page).toHaveTitle("New company");
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);

    const save = page.getByRole("button", { name: /create new company/i });
    await expect(save).toBeVisible({
      visible: submitButton
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
      /manager/i
    ];
    for (const field of fields) {
      await expect(page.getByLabel(field)).toBeVisible({ visible });
    }
  }
});

test.describe("view company", () => {
  test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
    const user = await runAsDefaultUser({ claims: [claims.company.any.view] });
    const company = await createCompany({
      managerId: user.id
    });

    await page.goto(routes.companies.view(company.id));

    await expectMinimalUi(page, company);
  });

  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(routes.companies.view(company.id));

    await expectMinimalUi(page, company, {
      title: "minimal",
      companyFields: false,
      forbidden: true,
      notFound: false
    });
  });

  test("should be able to click edit company button", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.update]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(company.id));

    await expectMinimalUi(page, company, { editButton: true });

    const button = page.getByRole("link", { name: /edit/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.edit(company.id));
  });

  test("should be able to click delete company button", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(company.id));

    await expectMinimalUi(page, company, { deleteButton: true });

    const button = page.getByRole("link", { name: /delete/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.delete(company.id));
  });

  test("should see not found", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({ claims: [claims.company.any.view] });
    await page.goto(routes.companies.view("1"));

    await expectMinimalUi(page, undefined, {
      title: "minimal",
      companyFields: false,
      forbidden: false,
      notFound: true
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
      notFound = false
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
      visible: forbidden
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);
    if (company) {
      await expectCompanyFieldsToValues(page, company, companyFields);
    }
    await expect(page.getByRole("link", { name: /edit/i })).toBeVisible({
      visible: editButton
    });
    await expect(page.getByRole("link", { name: /delete/i })).toBeVisible({
      visible: deleteButton
    });
    await expect(page.getByText(/company not found/i)).toBeVisible({
      visible: notFound
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
      "manager"
    ];

    for (const field of fields) {
      await expect(page.getByLabel(field)).toBeVisible({
        visible
      });
    }
  }

  async function expectCompanyFieldsToValues(
    page: Page,
    company: Company,
    visible = true
  ) {
    const fields = [
      { key: "name", value: company.name },
      { key: "address", value: company.address },
      { key: "ceo", value: company.ceo },
      { key: "contacts", value: company.contacts },
      { key: "email", value: company.email },
      { key: "inn", value: company.inn },
      { key: "phone", value: company.phone },
      { key: "type", value: company.type },
      {
        key: "manager",
        value: `${company.manager?.firstName} ${company.manager?.lastName}`
      }
    ];

    for (const field of fields) {
      const element = page.getByLabel(field.key);
      await expect(element).toBeVisible({ visible });
      if (visible) {
        await expect(element).toHaveText(field.value);
      }
    }
  }
});

test.describe("edit company", () => {
  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(routes.companies.edit(company.id));

    await expectMinimalUi(page, company, {
      forbidden: true,
      companyFields: false,
      submitButton: false,
      title: "minimal"
    });
  });

  test("should be able to edit company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.update]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.edit(company.id));

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

    await expect(page).toHaveURL(routes.companies.view(company.id));
  });

  test("should see not found", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({ claims: [claims.company.any.update] });
    await page.goto(routes.companies.edit(1));

    await expectMinimalUi(page, undefined, {
      companyFields: false,
      submitButton: false,
      title: "minimal",
      notFound: true
    });
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    submitButton?: boolean;
    title?: "minimal" | "full";
    notFound?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    company?: Company,
    {
      forbidden = false,
      companyFields = true,
      submitButton = true,
      title = "full",
      notFound = false
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("Edit company");
    } else {
      if (company) {
        await expect(page).toHaveTitle(company.name);
      }
    }
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden
    });
    await expectCompanyFieldsToBeVisible(page, companyFields);

    const save = page.getByRole("button", { name: /save changes/i });
    await expect(save).toBeVisible({
      visible: submitButton
    });

    await expect(page.getByText(/company not found/i)).toBeVisible({
      visible: notFound
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
      /type/i
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
    createCompany
  }) => {
    await runAsDefaultUser();
    const company = await createCompany();
    await page.goto(routes.companies.delete(company.id));

    await expectMinimalUi(page, company, {
      cancelButton: false,
      forbidden: true,
      okButton: false,
      title: "minimal"
    });
  });

  test("should be able to click delete company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.delete(company.id));

    await expectMinimalUi(page, company);

    const deleteButton = page.getByRole("button", {
      name: /delete company/i
    });
    await deleteButton.click();

    await expect(page).toHaveURL(routes.companies.index);
  });

  test("should be able to cancel", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const company = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.delete(company.id));

    await expectMinimalUi(page, company);

    const cancel = page.getByRole("link", { name: /cancel/i });
    await cancel.click();

    await expect(page).toHaveURL(routes.companies.view(company.id));
  });

  test("should see not found", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({ claims: [claims.company.any.delete] });
    await page.goto(routes.companies.delete(1));

    await expectMinimalUi(page, undefined, {
      cancelButton: false,
      okButton: false,
      title: "minimal",
      notFound: true
    });
  });

  type VisibilityOptions = {
    forbidden?: boolean;
    okButton?: boolean;
    cancelButton?: boolean;
    title?: "minimal" | "full";
    notFound?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    company?: Company,
    {
      forbidden = false,
      okButton = true,
      cancelButton = true,
      title = "full",
      notFound = false
    }: VisibilityOptions = {}
  ) {
    if (title === "minimal") {
      await expect(page).toHaveTitle("Delete company");
    } else {
      if (company) {
        await expect(page).toHaveTitle(`${company.name} • Delete`);
      }
    }
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden
    });

    const deleteButtonElem = page.getByRole("button", {
      name: /delete company/i
    });
    await expect(deleteButtonElem).toBeVisible({
      visible: okButton
    });

    const cancelButtonElem = page.getByRole("link", { name: /cancel/i });
    await expect(cancelButtonElem).toBeVisible({
      visible: cancelButton
    });

    await expect(page.getByText(/company not found/i)).toBeVisible({
      visible: notFound
    });
  }
});
