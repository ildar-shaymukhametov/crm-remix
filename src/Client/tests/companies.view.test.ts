import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("minimal ui", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser();
  const id = await createCompany();
  await page.goto(routes.companies.index);

  await expectMinimalUi(page, [await getCompany(id)], {
    noCompaniesFound: true
  });
});

test("clicks new company butt", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({ claims: [claims.company.create] });
  const id = await createCompany();
  await page.goto(routes.companies.index);

  await expectMinimalUi(page, [await getCompany(id)], {
    newCompanyButton: true,
    noCompaniesFound: true
  });

  const link = page.getByRole("link", { name: /new company/i });
  await link.click();
  await expect(page).toHaveURL(routes.companies.new);
});

test("does not see own company without claim", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser();

  const id = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.index);

  await expectMinimalUi(page, [await getCompany(id)], {
    noCompaniesFound: true
  });
});

test.describe("edit button", () => {
  for (const claim of [
    claims.company.any.manager.setFromAnyToAny,
    claims.company.any.manager.setFromAnyToNone,
    claims.company.any.manager.setFromAnyToSelf,
    claims.company.any.manager.setFromNoneToAny,
    claims.company.any.manager.setFromNoneToSelf
  ]) {
    test(`clicks in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        managerField: true
      });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });
  }

  test(`clicks in non-owned company with claim ${claims.company.any.name.set}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.name.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      editCompanyButton: true,
      nameField: true
    });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(id));
  });

  test(`clicks in non-owned company with claim ${claims.company.any.other.set}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.other.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      editCompanyButton: true,
      otherFields: true
    });

    const link = page.getByRole("link", { name: /edit company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.edit(id));
  });

  for (const claim of [
    claims.company.any.other.set,
    claims.company.whereUserIsManager.other.set
  ]) {
    test(`clicks in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        otherFields: true
      });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });
  }

  for (const claim of [
    claims.company.any.name.set,
    claims.company.whereUserIsManager.name.set
  ]) {
    test(`clicks in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        nameField: true
      });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromSelfToAny,
    claims.company.any.manager.setFromSelfToNone,
    claims.company.whereUserIsManager.manager.setFromSelfToAny,
    claims.company.whereUserIsManager.manager.setFromSelfToNone
  ]) {
    test(`clicks in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        managerField: true
      });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });
  }
});

test.describe("delete button", () => {
  test("clicks in own company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.delete]
    });

    const id = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      deleteCompanyButton: true
    });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(id));
  });

  test("clicks in non-owned company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.delete]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      deleteCompanyButton: true
    });

    const link = page.getByRole("link", { name: /delete company/i });
    await link.click();
    await expect(page).toHaveURL(routes.companies.delete(id));
  });
});

test.describe("name field", () => {
  test(`sees in non-owned company with claim ${claims.company.any.name.get}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.name.get]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      nameField: true
    });
  });

  test(`sees in non-owned company with claim ${claims.company.any.name.set}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.name.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      nameField: true,
      editCompanyButton: true
    });
  });

  for (const claim of [
    claims.company.any.name.get,
    claims.company.whereUserIsManager.name.get
  ]) {
    test(`sees in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        nameField: true
      });
    });
  }

  for (const claim of [
    claims.company.any.name.set,
    claims.company.whereUserIsManager.name.set
  ]) {
    test(`sees in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        nameField: true,
        editCompanyButton: true
      });
    });
  }

  for (const claim of [
    claims.company.whereUserIsManager.name.get,
    claims.company.whereUserIsManager.name.set
  ]) {
    test(`does not see in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        noCompaniesFound: true
      });
    });
  }
});

test.describe("other fields", () => {
  test(`sees in non-owned company with claim ${claims.company.any.other.get}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.other.get]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      otherFields: true
    });
  });

  test(`sees in non-owned company with claim ${claims.company.any.other.set}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.other.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      otherFields: true,
      editCompanyButton: true
    });
  });

  for (const claim of [
    claims.company.any.other.get,
    claims.company.whereUserIsManager.other.get
  ]) {
    test(`sees in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        otherFields: true
      });
    });
  }

  for (const claim of [
    claims.company.any.other.set,
    claims.company.whereUserIsManager.other.set
  ]) {
    test(`sees in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        otherFields: true,
        editCompanyButton: true
      });
    });
  }

  for (const claim of [
    claims.company.whereUserIsManager.other.get,
    claims.company.whereUserIsManager.other.set
  ]) {
    test(`does not see in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        noCompaniesFound: true
      });
    });
  }
});

test.describe("manager", () => {
  test(`sees in non-owned company with claim ${claims.company.any.manager.get}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.manager.get]
    });

    const id = await createCompany();
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      managerField: true
    });
  });

  for (const claim of [
    claims.company.any.manager.setFromAnyToAny,
    claims.company.any.manager.setFromAnyToNone,
    claims.company.any.manager.setFromAnyToSelf,
    claims.company.any.manager.setFromNoneToAny,
    claims.company.any.manager.setFromNoneToSelf
  ]) {
    test(`sees in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        managerField: true,
        editCompanyButton: true
      });
    });
  }

  test(`sees in own company with claim ${claims.company.whereUserIsManager.manager.get}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.manager.get]
    });

    const id = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.index);

    await expectMinimalUi(page, [await getCompany(id)], {
      managerField: true
    });
  });

  for (const claim of [
    claims.company.any.manager.setFromSelfToAny,
    claims.company.any.manager.setFromSelfToNone,
    claims.company.whereUserIsManager.manager.setFromSelfToAny,
    claims.company.whereUserIsManager.manager.setFromSelfToNone
  ]) {
    test(`sees in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        managerField: true,
        editCompanyButton: true
      });
    });
  }

  for (const claim of [
    claims.company.whereUserIsManager.manager.get,
    claims.company.whereUserIsManager.manager.setFromSelfToAny,
    claims.company.whereUserIsManager.manager.setFromSelfToNone
  ]) {
    test(`does not see in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        noCompaniesFound: true
      });
    });
  }
});

type VisibilityOptions = {
  newCompanyButton?: boolean;
  noCompaniesFound?: boolean;
  editCompanyButton?: boolean;
  deleteCompanyButton?: boolean;
  nameField?: boolean;
  otherFields?: boolean;
  managerField?: boolean;
};

async function expectMinimalUi(
  page: Page,
  companies: Company[],
  {
    newCompanyButton = false,
    noCompaniesFound = false,
    editCompanyButton = false,
    deleteCompanyButton = false,
    nameField = false,
    otherFields = false,
    managerField = false
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

  for (const company of companies) {
    const fields = [
      { key: /name/i, value: company?.fields.name, visible: nameField },
      {
        key: /address/i,
        value: company?.fields.address,
        visible: otherFields
      },
      { key: /ceo/i, value: company?.fields.ceo, visible: otherFields },
      {
        key: /contacts/i,
        value: company?.fields.Contacts,
        visible: otherFields
      },
      { key: /email/i, value: company?.fields.email, visible: otherFields },
      { key: /inn/i, value: company?.fields.inn, visible: otherFields },
      { key: /phone/i, value: company?.fields.phone, visible: otherFields },
      {
        key: /type/i,
        value: (company?.fields.type as CompanyType)?.name,
        visible: otherFields
      },
      {
        key: /manager/i,
        value: company?.fields.manager
          ? `${(company?.fields.manager as Manager)?.lastName} ${
              (company?.fields.manager as Manager)?.firstName
            }`
          : "-",
        visible: managerField
      }
    ];

    await expectFieldsToBeVisible(page, fields);
  }
}

async function expectFieldsToBeVisible(
  page: Page,
  fields: {
    key: RegExp;
    value: object | string | undefined;
    visible: boolean;
  }[]
) {
  for (const field of fields) {
    const element = page.getByLabel(field.key);
    await expect(element).toBeVisible({
      visible: field.visible
    });

    if (field.visible) {
      await expect(element).toHaveText(field.value?.toString() ?? "");
    }
  }
}
