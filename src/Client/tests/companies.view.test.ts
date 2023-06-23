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

type TestData = {
  claims: string[];
  options: VisibilityOptions;
};

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
  const testData: Array<TestData> = [
    { claims: [claims.company.any.other.set], options: {} },
    { claims: [claims.company.any.name.set], options: { nameField: true } },
    { claims: [claims.company.any.manager.setFromAnyToAny], options: {} },
    { claims: [claims.company.any.manager.setFromAnyToNone], options: {} },
    { claims: [claims.company.any.manager.setFromAnyToSelf], options: {} },
    { claims: [claims.company.any.manager.setFromNoneToAny], options: {} },
    { claims: [claims.company.any.manager.setFromNoneToSelf], options: {} }
  ];

  for (const data of testData) {
    test(`clicks in non-owned company with claim ${data.claims[0]}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: data.claims
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        ...data.options
      });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });
  }

  const seedData2: Array<TestData> = [
    { claims: [claims.company.any.other.set], options: {} },
    { claims: [claims.company.whereUserIsManager.other.set], options: {} },
    { claims: [claims.company.any.name.set], options: { nameField: true } },
    {
      claims: [claims.company.whereUserIsManager.name.set],
      options: { nameField: true }
    },
    { claims: [claims.company.any.manager.setFromSelfToAny], options: {} },
    {
      claims: [claims.company.whereUserIsManager.manager.setFromSelfToAny],
      options: {}
    },
    { claims: [claims.company.any.manager.setFromSelfToNone], options: {} },
    {
      claims: [claims.company.whereUserIsManager.manager.setFromSelfToNone],
      options: {}
    }
  ];

  for (const data of seedData2) {
    test(`clicks in own company with claim ${data.claims[0]}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: data.claims
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        editCompanyButton: true,
        ...data.options
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
  const testData: Array<TestData> = [
    { claims: [claims.company.any.name.get], options: { nameField: true } },
    {
      claims: [claims.company.any.name.set],
      options: { nameField: true, editCompanyButton: true }
    }
  ];

  for (const data of testData) {
    test(`sees in non-owned company with claim ${data.claims[0]}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: data.claims
      });

      const id = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        nameField: true,
        ...data.options
      });
    });
  }

  const testData2: Array<TestData> = [
    { claims: [claims.company.any.name.get], options: { nameField: true } },
    {
      claims: [claims.company.any.name.set],
      options: { nameField: true, editCompanyButton: true }
    },
    {
      claims: [claims.company.whereUserIsManager.name.get],
      options: { nameField: true }
    },
    {
      claims: [claims.company.whereUserIsManager.name.set],
      options: { nameField: true, editCompanyButton: true }
    }
  ];

  for (const data of testData2) {
    test(`sees in own company with claim ${data.claims[0]}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: data.claims
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, [await getCompany(id)], {
        nameField: true,
        ...data.options
      });
    });
  }

  const testData3: Array<TestData> = [
    {
      claims: [claims.company.whereUserIsManager.name.get],
      options: {}
    },
    {
      claims: [claims.company.whereUserIsManager.name.set],
      options: {}
    }
  ];

  for (const data of testData3) {
    test(`does not see in non-owned company with claim ${data.claims[0]}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: data.claims
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
      { key: /name/i, value: company?.fields.Name, visible: nameField },
      {
        key: /address/i,
        value: company?.fields.Address,
        visible: otherFields
      },
      { key: /ceo/i, value: company?.fields.Ceo, visible: otherFields },
      {
        key: /contacts/i,
        value: company?.fields.Contacts,
        visible: otherFields
      },
      { key: /email/i, value: company?.fields.Email, visible: otherFields },
      { key: /inn/i, value: company?.fields.Inn, visible: otherFields },
      { key: /phone/i, value: company?.fields.Phone, visible: otherFields },
      {
        key: /type/i,
        value: (company?.fields.Type as CompanyType)?.name,
        visible: otherFields
      },
      {
        key: /manager/i,
        value: company?.fields.Manager
          ? `${(company?.fields.Manager as Manager)?.lastName} ${
              (company?.fields.Manager as Manager)?.firstName
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
