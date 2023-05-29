import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";

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

  test("should be able to click edit company button in any company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.update]
    });

    const companyId = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { editCompanyButton: true });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button in any company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });

    const companyId = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { deleteCompanyButton: true });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(companyId));
  });

  test("should be able to click edit company button in own company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.update]
    });

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { editCompanyButton: true });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button in own company", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.delete]
    });

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { deleteCompanyButton: true });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(companyId));
  });

  test("should not be able to view own company without claim", async ({
    page,
    runAsDefaultUser,
    createCompany
  }) => {
    const user = await runAsDefaultUser();

    await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, { noCompaniesFound: true });
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

test.describe("view company", () => {
  test("should be able to view any company", async ({
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

  test("should be forbidden if no claims", async ({
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

  test("should be able to click edit button in any company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.update]
    });

    const companyId = await createCompany();
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { editButton: true });

    const button = page.getByRole("link", { name: /edit/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button in any company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });

    const companyId = await createCompany();
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

  for (const claim of [claims.company.whereUserIsManager.view]) {
    test(`should be able to view own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });
      const companyId = await createCompany({
        managerId: user.id
      });

      await page.goto(routes.companies.view(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company);
    });
  }

  for (const claim of [
    claims.company.whereUserIsManager.view,
    claims.company.whereUserIsManager.delete,
    claims.company.whereUserIsManager.update
  ]) {
    test(`should not be able to view non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });
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
  }

  test(`should not be able to view own company without claim`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser();
    const companyId = await createCompany({
      managerId: user.id
    });

    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, {
      title: "minimal",
      companyFields: false,
      forbidden: true,
      notFound: false
    });
  });

  test(`should be able to click edit button in own company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.update]
    });

    const companyId = await createCompany({
      managerId: user.id
    });

    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { editButton: true });

    const button = page.getByRole("link", { name: /edit/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.edit(companyId));
  });

  test("should be able to click delete company button in own company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.delete]
    });

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { deleteButton: true });

    const button = page.getByRole("link", { name: /delete/i });
    await button.click();
    await expect(page).toHaveURL(routes.companies.delete(companyId));
  });

  test("should not be able to click delete company button in own company without claim", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser();

    const companyId = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);

    await expectMinimalUi(page, company, {
      title: "minimal",
      companyFields: false,
      forbidden: true,
      notFound: false
    });
  });

  test("should not be able to click delete company button in non-owned company", async ({
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
