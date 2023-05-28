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
    runAsDefaultUser,
    createCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.create] });
    await createCompany();
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

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { editCompanyButton: true });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { deleteCompanyButton: true });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(companyId));
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
    await runAsDefaultUser({
      claims: [claims.company.create, claims.company.any.view]
    });
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
    const typeName =
      (await type.getByRole("option", { selected: true }).textContent()) ?? "";

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();
    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));

    await expect(page.getByLabel(/type/i)).toHaveText(typeName);
  });

  for (const claim of [
    claims.company.any.setManagerFromAnyToAny,
    claims.company.any.setManagerFromAnyToSelf,
    claims.company.any.setManagerFromNoneToSelf,
    claims.company.any.setManagerFromNoneToAny
  ]) {
    test(`should be able to set manager to self with claim ${claim}`, async ({
      page,
      runAsDefaultUser
    }) => {
      const user = await runAsDefaultUser({
        claims: [claims.company.create, claims.company.any.view, claim]
      });

      await page.goto(routes.companies.new);
      await expectMinimalUi(page, { manager: true });

      const company = buildCompany();
      await page.getByLabel(/name/i).fill(company.name);

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /create new company/i });
      await submit.click();

      await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));

      const fullName = `${user.firstName} ${user.lastName}`;
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromAnyToAny,
    claims.company.any.setManagerFromNoneToAny
  ]) {
    test(`should be able to set manager to any with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.create, claims.company.any.view, claim]
      });
      const user = await createUser();

      await page.goto(routes.companies.new);
      await expectMinimalUi(page, { manager: true });

      const company = buildCompany();
      await page.getByLabel(/name/i).fill(company.name);

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /create new company/i });
      await submit.click();

      await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));

      const fullName = `${user.firstName} ${user.lastName}`;
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    submitButton?: boolean;
    manager?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    {
      forbidden = false,
      companyFields = true,
      submitButton = true,
      manager = false
    }: VisibilityOptions = {}
  ) {
    await expect(page).toHaveTitle("New company");
    await expect(page.getByText(/forbidden/i)).toBeVisible({
      visible: forbidden
    });

    await expectCompanyFieldsToBeVisible(page, {
      name: companyFields,
      address: companyFields,
      ceo: companyFields,
      contacts: companyFields,
      email: companyFields,
      inn: companyFields,
      phone: companyFields,
      type: companyFields,
      manager
    });

    const save = page.getByRole("button", { name: /create new company/i });
    await expect(save).toBeVisible({
      visible: submitButton
    });
  }

  async function expectCompanyFieldsToBeVisible(
    page: Page,
    data: { [key: string]: boolean }
  ) {
    for (const key of Object.keys(data)) {
      await expect(page.getByLabel(key)).toBeVisible({ visible: data[key] });
    }
  }
});

test.describe.only("view company", () => {
  test("user can view any company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.any.view] });
    const companyId = await createCompany();

    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company);
  });

  test("user can view own company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({ claims: [claims.company.whereUserIsManager.view] });
    const companyId = await createCompany({
      managerId: user.id
    });

    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company);
  });

  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser();
    const companyId = await createCompany();
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
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
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.update]
    });
    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { editButton: true });

    const button = page.getByRole("link", { name: /edit/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { deleteButton: true });

    const button = page.getByRole("link", { name: /delete/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.delete(companyId));
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
    if (company && companyFields) {
      await expectCompanyFieldsToValues(page, company);
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

  async function expectCompanyFieldsToValues(page: Page, company: Company) {
    const fields = [
      { key: "name", value: company.name },
      { key: "address", value: company.address },
      { key: "ceo", value: company.ceo },
      { key: "contacts", value: company.contacts },
      { key: "email", value: company.email },
      { key: "inn", value: company.inn },
      { key: "phone", value: company.phone },
      { key: "type", value: company.type?.name },
      {
        key: "manager",
        value: company.manager
          ? `${company.manager?.firstName} ${company.manager?.lastName}`
          : "-"
      }
    ];

    for (const field of fields) {
      const element = page.getByLabel(field.key);
      await expect(element).toHaveText(field.value);
    }
  }
});

test.describe("edit company", () => {
  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser();
    const companyId = await createCompany();
    await page.goto(routes.companies.edit(companyId));

    const company = await getCompany(companyId);
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
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.update]
    });
    const companyId = await createCompany();
    await page.goto(routes.companies.edit(companyId));

    const company = await getCompany(companyId);
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
    await expect(selectedOption).toHaveText(company.type?.name);

    const newCompany = buildCompany();
    await name.fill(newCompany.name);
    await address.fill(newCompany.address);
    await ceo.fill(newCompany.ceo);
    await contacts.fill(newCompany.contacts);
    await email.fill(newCompany.email);
    await inn.fill(newCompany.inn);
    await phone.fill(newCompany.phone);
    await type.selectOption(newCompany.typeId ?? null);
    const typeName =
      (await type.getByRole("option", { selected: true }).textContent()) ?? "";

    const submit = page.getByRole("button", { name: /save changes/i });
    await submit.click();

    await expect(page).toHaveURL(routes.companies.view(companyId));

    await expect(page.getByLabel(/type/i)).toHaveText(typeName);
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

  for (const claim of [
    claims.company.any.setManagerFromNoneToSelf,
    claims.company.any.setManagerFromNoneToAny,
    claims.company.any.setManagerFromAnyToSelf,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from none to self with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claims.company.any.update, claim]
      });

      const companyId = await createCompany();
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));

      const fullName = `${user.firstName} ${user.lastName}`;
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromNoneToAny,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from none to any with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const user = await createUser();

      const companyId = await createCompany();
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));

      const fullName = `${user.firstName} ${user.lastName}`;
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromSelfToAny,
    claims.company.any.setManagerFromSelfToNone,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from self to none with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.firstName} ${user.lastName}`
      );

      await manager.selectOption("");

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));
      await expect(page.getByLabel(/manager/i)).toHaveText("-");
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromAnyToNone,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from any to none with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const user = await createUser();

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.firstName} ${user.lastName}`
      );

      await manager.selectOption("");

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));
      await expect(page.getByLabel(/manager/i)).toHaveText("-");
    });
  }

  for (const claim of [claims.company.any.setManagerFromAnyToAny]) {
    test(`should be able to set manager from any to any with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const user = await createUser();
      const user2 = await createUser();

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.firstName} ${user.lastName}`
      );

      const fullName = `${user2.firstName} ${user2.lastName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromSelfToAny,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from self to any with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      const user = await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const anotherUser = await createUser();

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.firstName} ${user.lastName}`
      );

      const fullName = `${anotherUser.firstName} ${anotherUser.lastName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.setManagerFromAnyToSelf,
    claims.company.any.setManagerFromAnyToAny
  ]) {
    test(`should be able to set manager from any to self with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      const user = await runAsDefaultUser({
        claims: [claims.company.any.update, claims.company.any.view, claim]
      });

      const anotherUser = await createUser();

      const companyId = await createCompany({ managerId: anotherUser.id });
      await page.goto(routes.companies.edit(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { manager: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${anotherUser.firstName} ${anotherUser.lastName}`
      );

      const fullName = `${user.firstName} ${user.lastName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(companyId));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  type VisibilityOptions = {
    forbidden?: boolean;
    companyFields?: boolean;
    submitButton?: boolean;
    title?: "minimal" | "full";
    notFound?: boolean;
    manager?: boolean;
  };

  async function expectMinimalUi(
    page: Page,
    company?: Company,
    {
      forbidden = false,
      companyFields = true,
      submitButton = true,
      title = "full",
      notFound = false,
      manager = false
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

    await expectCompanyFieldsToBeVisible(page, {
      name: companyFields,
      address: companyFields,
      ceo: companyFields,
      contacts: companyFields,
      email: companyFields,
      inn: companyFields,
      phone: companyFields,
      type: companyFields,
      manager
    });

    const save = page.getByRole("button", { name: /save changes/i });
    await expect(save).toBeVisible({
      visible: submitButton
    });

    await expect(page.getByText(/company not found/i)).toBeVisible({
      visible: notFound
    });
  }

  async function expectCompanyFieldsToBeVisible(
    page: Page,
    data: { [key: string]: boolean }
  ) {
    for (const key of Object.keys(data)) {
      await expect(page.getByLabel(key)).toBeVisible({ visible: data[key] });
    }
  }
});

test.describe("delete company", () => {
  test("should be forbidden", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser();
    const companyId = await createCompany();
    await page.goto(routes.companies.delete(companyId));

    const company = await getCompany(companyId);
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
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.delete(companyId));

    const company = await getCompany(companyId);
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
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });
    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.delete(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company);

    const cancel = page.getByRole("link", { name: /cancel/i });
    await cancel.click();

    await expect(page).toHaveURL(routes.companies.view(companyId));
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
