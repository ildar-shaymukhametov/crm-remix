import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("forbidden if no claims", async ({
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

test("not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.other.get] });
  await page.goto(routes.companies.view("1"));

  await expectMinimalUi(page, undefined, {
    title: "minimal",
    notFound: true
  });
});

test.describe("other fields", () => {
  test(`sees in non-owned company with claim ${claims.company.any.other.get}`, async ({
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

  test(`sees in non-owned company with claim ${claims.company.any.other.set}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.any.other.set] });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      otherFields: true,
      editButton: true
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
      const user = await runAsDefaultUser({ claims: [claim] });
      const id = await createCompany({ managerId: user.id });

      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { otherFields: true });
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
      const user = await runAsDefaultUser({ claims: [claim] });
      const id = await createCompany({ managerId: user.id });

      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        otherFields: true,
        editButton: true
      });
    });
  }

  test("forbidden to view in non-owned company", async ({
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
    test(`clicks in non-owned company`, async ({
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

    test(`forbidden in non-owned company`, async ({
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
      test(`clicks in own company with claim ${claim}`, async ({
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
      test(`forbidden to click in non-owned company with claim ${claim}`, async ({
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
      test(`clicks in own company with claim ${claim}`, async ({
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

  test.describe("name, title", () => {
    test(`clicks in non-owned company`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.any.name.set]
      });

      const id = await createCompany();
      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        editButton: true,
        nameField: true,
        title: "full"
      });

      const button = page.getByRole("link", { name: /edit/i });
      await button.click();
      await expect(page).toHaveURL(routes.companies.edit(id));
    });

    test(`forbidden in non-owned company`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      await runAsDefaultUser({
        claims: [claims.company.whereUserIsManager.name.set]
      });

      const id = await createCompany();
      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        forbidden: true
      });
    });

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

        const companyId = await createCompany({
          managerId: user.id
        });

        await page.goto(routes.companies.view(companyId));

        const company = await getCompany(companyId);
        await expectMinimalUi(page, company, {
          editButton: true,
          nameField: true,
          title: "full"
        });

        const button = page.getByRole("link", { name: /edit/i });
        await button.click();
        await expect(page).toHaveURL(routes.companies.edit(companyId));
      });
    }
  });
});

test.describe("delete button", () => {
  test("clicks in non-owned company", async ({
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

  test("forbidden in non-owned company", async ({
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
    test(`clicks in own company with claim ${claim}`, async ({
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

test.describe("manager", () => {
  test(`sees in non-owned company with claim`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.any.manager.get] });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { managerField: true });
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
      await runAsDefaultUser({ claims: [claim] });
      const id = await createCompany();

      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        managerField: true,
        editButton: true
      });
    });
  }

  test(`forbidden to view in non-owned company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.manager.get]
    });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { forbidden: true });
  });

  test(`sees in own company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.manager.get]
    });
    const id = await createCompany({ managerId: user.id });

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { managerField: true });
  });

  for (const claim of [
    claims.company.any.manager.setFromAnyToAny,
    claims.company.any.manager.setFromAnyToNone,
    claims.company.any.manager.setFromAnyToSelf,
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

      await page.goto(routes.companies.view(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, {
        managerField: true,
        editButton: true
      });
    });
  }
});

test.describe("name, title", () => {
  test(`sees in non-owned company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({ claims: [claims.company.any.name.get] });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { nameField: true, title: "full" });
  });

  test(`forbidden to view in non-owned company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.name.get]
    });
    const id = await createCompany();

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { forbidden: true });
  });

  test(`sees in own company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.name.get]
    });
    const id = await createCompany({ managerId: user.id });

    await page.goto(routes.companies.view(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { nameField: true, title: "full" });
  });
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
