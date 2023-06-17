import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("should be forbidden if no claims", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser();
  const id = await createCompany();
  await page.goto(routes.companies.view(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    title: "minimal",
    forbidden: true
  });
});

test("should see not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.other.get] });
  await page.goto(routes.companies.view("1"));

  await expectMinimalUi(page, undefined, {
    title: "minimal",
    otherFields: false,
    forbidden: false,
    notFound: true
  });
});

test.describe("other fields", () => {
  test(`should be able to view in non-owned company}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.any.other.get] });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { otherFields: true });
  });

  for (const claim of [
    claims.company.any.other.get,
    claims.company.whereUserIsManager.other.get
  ]) {
    test(`should be able to view in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({ claims: [claim] });
      const id = await createCompany({ managerId: user.id });

      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { otherFields: true });
    });
  }

  test("should be forbidden to view in non-owned company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.other.get]
    });
    const id = await createCompany();
    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      title: "minimal",
      forbidden: true
    });
  });
});

test.describe("edit button", () => {
  test.describe("other fields", () => {
    test(`should be able to click in non-owned company`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.any.other.set]
      });

      const id = await createCompany();
      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        editButton: true,
        otherFields: true
      });

      const button = page.getByRole("link", { name: /edit/i });
      await button.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });

    test(`should be forbidden in non-owned company`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.whereUserIsManager.other.set]
      });

      const id = await createCompany();
      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        title: "minimal",
        forbidden: true
      });
    });

    for (const claim of [
      claims.company.any.other.set,
      claims.company.whereUserIsManager.other.set
    ]) {
      test(`should be able to click in own company with claim ${claim}`, async ({
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
        await expectMinimalUi(page, company, {
          editButton: true,
          otherFields: true
        });

        const button = page.getByRole("link", { name: /edit/i });
        await button.click();
        await expect(page).toHaveURL(routes.companies.edit(companyId));
      });
    }
  });

  test.describe("manager", () => {
    for (const claim of [
      claims.company.any.manager.setFromAnyToAny,
      claims.company.any.manager.setFromAnyToNone,
      claims.company.any.manager.setFromAnyToSelf,
      claims.company.any.manager.setFromNoneToAny,
      claims.company.any.manager.setFromNoneToSelf
    ]) {
      test(`should be able to click in non-owned company with claim ${claim}`, async ({
        page,
        runAsDefaultUser,
        createCompany,
        getCompany
      }) => {
        await runAsDefaultUser({
          claims: [claim]
        });

        const id = await createCompany();
        await page.goto(routes.companies.view(id));

        const company = await getCompany(id);
        await expectMinimalUi(page, company, {
          editButton: true,
          managerField: true
        });

        const button = page.getByRole("link", { name: /edit/i });
        await button.click();
        await expect(page).toHaveURL(routes.companies.edit(id));
      });
    }

    for (const claim of [
      claims.company.whereUserIsManager.manager.setFromSelfToAny,
      claims.company.whereUserIsManager.manager.setFromSelfToNone
    ]) {
      test(`should be forbidden to click in non-owned company with claim ${claim}`, async ({
        page,
        runAsDefaultUser,
        createCompany,
        getCompany
      }) => {
        await runAsDefaultUser();
        const id = await createCompany();
        await page.goto(routes.companies.view(id));

        const company = await getCompany(id);
        await expectMinimalUi(page, company, {
          title: "minimal",
          forbidden: true
        });
      });
    }

    for (const claim of [
      claims.company.any.manager.setFromAnyToAny,
      claims.company.any.manager.setFromAnyToNone,
      claims.company.any.manager.setFromAnyToSelf,
      claims.company.any.manager.setFromSelfToAny,
      claims.company.any.manager.setFromSelfToNone,
      claims.company.whereUserIsManager.manager.setFromSelfToAny,
      claims.company.whereUserIsManager.manager.setFromSelfToNone
    ]) {
      test(`should be able to click in own company with claim ${claim}`, async ({
        page,
        runAsDefaultUser,
        createCompany,
        getCompany
      }) => {
        const user = await runAsDefaultUser({
          claims: [claim]
        });

        const id = await createCompany({
          managerId: user.id
        });
        await page.goto(routes.companies.view(id));

        const company = await getCompany(id);
        await expectMinimalUi(page, company, {
          editButton: true,
          managerField: true
        });

        const button = page.getByRole("link", { name: /edit/i });
        await button.click();
        await expect(page).toHaveURL(routes.companies.edit(id));
      });
    }
  });
});

test.describe("delete button", () => {
  test("should be able to click in non-owned company", async ({
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

  test("should be forbidden in non-owned company", async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.delete]
    });

    const companyId = await createCompany();
    await page.goto(routes.companies.view(companyId));

    const company = await getCompany(companyId);
    await expectMinimalUi(page, company, { forbidden: true, title: "minimal" });
  });

  for (const claim of [
    claims.company.any.delete,
    claims.company.whereUserIsManager.delete
  ]) {
    test(`should be able to click in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.view(companyId));

      const company = await getCompany(companyId);
      await expectMinimalUi(page, company, { deleteButton: true });

      const button = page.getByRole("link", { name: /delete/i });
      await button.click();
      await expect(page).toHaveURL(routes.companies.delete(companyId));
    });
  }
});

type VisibilityOptions = {
  forbidden?: boolean;
  otherFields?: boolean;
  title?: "minimal" | "full";
  editButton?: boolean;
  deleteButton?: boolean;
  notFound?: boolean;
  managerField?: boolean;
  nameField?: boolean;
};

async function expectMinimalUi(
  page: Page,
  company?: Company,
  {
    forbidden = false,
    otherFields = false,
    title = "minimal",
    editButton = false,
    deleteButton = false,
    notFound = false,
    managerField = false,
    nameField = false
  }: VisibilityOptions = {}
) {
  if (title === "minimal") {
    await expect(page).toHaveTitle("View company");
  } else {
    if (company && nameField) {
      await expect(page).toHaveTitle(company.fields?.Name?.toString());
    }
  }

  await expect(page.getByText(/forbidden/i)).toBeVisible({
    visible: forbidden
  });

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
