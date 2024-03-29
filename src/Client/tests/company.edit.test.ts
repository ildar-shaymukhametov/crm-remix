import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("forbidden if non-owned company and no claims", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser();
  const id = await createCompany();
  await page.goto(routes.companies.edit(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    forbidden: true,
    submitButton: false
  });
});

test("forbidden if own company but no claims", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser();
  const id = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.edit(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    forbidden: true,
    submitButton: false
  });
});

test("not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.name.set] });
  await page.goto(routes.companies.edit(1));

  await expectMinimalUi(page, undefined, {
    submitButton: false,
    notFound: true
  });
});

test.describe("name", () => {
  test(`sets in non-owned company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.name.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { nameField: true, title: "full" });

    const newCompanyData = buildCompany();
    await page.getByLabel(/name/i).fill(newCompanyData.name);

    const submit = page.getByRole("button", { name: /save changes/i });
    await submit.click();

    await expect(page).toHaveURL(routes.companies.view(id));
    await expect(page.getByLabel(/name/i)).toHaveText(newCompanyData.name);
  });

  test(`sets in own company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.name.set]
    });

    const id = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, { nameField: true, title: "full" });

    const newCompanyData = buildCompany();
    await page.getByLabel(/name/i).fill(newCompanyData.name);

    const submit = page.getByRole("button", { name: /save changes/i });
    await submit.click();

    await expect(page).toHaveURL(routes.companies.view(id));
    await expect(page.getByLabel(/name/i)).toHaveText(newCompanyData.name);
  });

  test(`forbidden`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.name.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      forbidden: true,
      submitButton: false,
      title: "minimal"
    });
  });
});

test.describe("other", () => {
  test(`sets in non-owned company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.any.other.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      otherFields: true,
      title: "minimal"
    });

    await assertUpdated(page, id);
  });

  test(`sets in own company`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.other.set]
    });

    const id = await createCompany({ managerId: user.id });
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      otherFields: true,
      title: "minimal"
    });

    await assertUpdated(page, id);
  });

  test(`forbidden`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.whereUserIsManager.other.set]
    });

    const id = await createCompany();
    await page.goto(routes.companies.edit(id));

    const company = await getCompany(id);
    await expectMinimalUi(page, company, {
      forbidden: true,
      submitButton: false,
      title: "minimal"
    });
  });

  async function assertUpdated(page: Page, id: number) {
    const newCompanyData = buildCompany();
    await page.getByLabel(/address/i).fill(newCompanyData.address);
    await page.getByLabel(/ceo/i).fill(newCompanyData.ceo);
    await page.getByLabel(/contacts/i).fill(newCompanyData.contacts);
    await page.getByLabel(/email/i).fill(newCompanyData.email);
    await page.getByLabel(/inn/i).fill(newCompanyData.inn);
    await page.getByLabel(/phone/i).fill(newCompanyData.phone);
    const type = page.getByLabel(/type/i);
    await type.selectOption(newCompanyData.typeId?.toString() ?? null);
    const expectedTypeName =
      (await type.getByRole("option", { selected: true }).textContent()) ?? "";

    const submit = page.getByRole("button", { name: /save changes/i });
    await submit.click();

    await expect(page).toHaveURL(routes.companies.view(id));
    await expect(page.getByLabel(/address/i)).toHaveText(
      newCompanyData.address
    );
    await expect(page.getByLabel(/ceo/i)).toHaveText(newCompanyData.ceo);
    await expect(page.getByLabel(/contacts/i)).toHaveText(
      newCompanyData.contacts
    );
    await expect(page.getByLabel(/email/i)).toHaveText(newCompanyData.email);
    await expect(page.getByLabel(/inn/i)).toHaveText(newCompanyData.inn);
    await expect(page.getByLabel(/phone/i)).toHaveText(newCompanyData.phone);
    await expect(page.getByLabel(/type/i)).toHaveText(expectedTypeName);
  }
});

test.describe("manager", () => {
  for (const claim of [
    claims.company.any.manager.setFromAnyToSelf,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from none to self in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const id = await createCompany();
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));

      await expect(page.getByLabel(/manager/i)).toHaveText(
        `${user.lastName} ${user.firstName}`
      );
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromNoneToSelf,
    claims.company.any.manager.setFromNoneToAny
  ]) {
    test(`sets from none to self in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim, claims.company.any.manager.get]
      });

      const id = await createCompany();
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));

      await expect(page.getByLabel(/manager/i)).toHaveText(
        `${user.lastName} ${user.firstName}`
      );
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromNoneToAny,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from none to any in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claim, claims.company.any.manager.get]
      });

      const user = await createUser();

      const id = await createCompany();
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        "-"
      );

      await manager.selectOption(user.id);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));

      await expect(page.getByLabel(/manager/i)).toHaveText(
        `${user.lastName} ${user.firstName}`
      );
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromSelfToAny,
    claims.company.any.manager.setFromSelfToNone,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from self to none in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim, claims.company.any.manager.get]
      });

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.lastName} ${user.firstName}`
      );

      await manager.selectOption("");

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));
      await expect(page.getByLabel(/manager/i)).toHaveText("-");
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromAnyToNone,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from any to none in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const user = await createUser();

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.lastName} ${user.firstName}`
      );

      await manager.selectOption("");

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));
      await expect(page.getByLabel(/manager/i)).toHaveText("-");
    });
  }

  for (const claim of [claims.company.any.manager.setFromAnyToAny]) {
    test(`sets from any to any in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const user = await createUser();
      const anotherUser = await createUser();

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.lastName} ${user.firstName}`
      );

      const fullName = `${anotherUser.lastName} ${anotherUser.firstName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromSelfToAny,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from self to any in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim, claims.company.any.manager.get]
      });

      const anotherUser = await createUser();

      const id = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${user.lastName} ${user.firstName}`
      );

      const fullName = `${anotherUser.lastName} ${anotherUser.firstName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }

  for (const claim of [
    claims.company.any.manager.setFromAnyToSelf,
    claims.company.any.manager.setFromAnyToAny
  ]) {
    test(`sets from any to self in any company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany,
      getCompany,
      createUser
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const anotherUser = await createUser();

      const id = await createCompany({ managerId: anotherUser.id });
      await page.goto(routes.companies.edit(id));

      const company = await getCompany(id);
      await expectMinimalUi(page, company, { managerField: true });

      const manager = page.getByLabel(/manager/i);
      await expect(manager.getByRole("option", { selected: true })).toHaveText(
        `${anotherUser.lastName} ${anotherUser.firstName}`
      );

      const fullName = `${user.lastName} ${user.firstName}`;
      await manager.selectOption(fullName);

      const submit = page.getByRole("button", { name: /save changes/i });
      await submit.click();

      await expect(page).toHaveURL(routes.companies.view(id));
      await expect(page.getByLabel(/manager/i)).toHaveText(fullName);
    });
  }
});

type VisibilityOptions = {
  forbidden?: boolean;
  otherFields?: boolean;
  submitButton?: boolean;
  title?: "minimal" | "full";
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
    submitButton = true,
    title = "minimal",
    notFound = false,
    managerField = false,
    nameField = false
  }: VisibilityOptions = {}
) {
  if (title === "minimal") {
    await expect(page).toHaveTitle("Edit company");
  } else {
    if (company?.fields?.name != undefined) {
      await expect(page).toHaveTitle(company.fields.name.toString());
    }
  }

  await expect(page.getByText(/forbidden/i)).toBeVisible({
    visible: forbidden
  });

  await expectOtherFieldsToBeVisible(page, {
    name: nameField,
    address: otherFields,
    ceo: otherFields,
    contacts: otherFields,
    email: otherFields,
    inn: otherFields,
    phone: otherFields,
    type: otherFields,
    manager: managerField
  });

  const save = page.getByRole("button", { name: /save changes/i });
  await expect(save).toBeVisible({
    visible: submitButton
  });

  await expect(page.getByText(/company not found/i)).toBeVisible({
    visible: notFound
  });
}

async function expectOtherFieldsToBeVisible(
  page: Page,
  data: { [key: string]: boolean }
) {
  for (const key of Object.keys(data)) {
    await expect(page.getByLabel(key)).toBeVisible({ visible: data[key] });
  }
}
