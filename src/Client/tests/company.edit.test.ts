import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { buildCompany, test } from "./companies-test";

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

test("should be forbidden if own company but no claims", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser();
  const companyId = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.edit(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company, {
    forbidden: true,
    companyFields: false,
    submitButton: false,
    title: "minimal"
  });
});

test("should be forbidden if non-owned company but has claim to edit other fields in own company", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.other.update]
  });

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

test("should be able to edit any field in any company", async ({
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
  const newData = await checkAndFillFields(page, company);

  const submit = page.getByRole("button", { name: /save changes/i });
  await submit.click();

  await expect(page).toHaveURL(routes.companies.view(companyId));
  await expect(page.getByLabel(/type/i)).toHaveText(newData.type.name);
});

test("should be able to edit any field in own company", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.other.update]
  });

  const companyId = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.edit(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company, { manager: true });
  const newData = await checkAndFillFields(page, company);

  const submit = page.getByRole("button", { name: /save changes/i });
  await submit.click();

  await expect(page).toHaveURL(routes.companies.view(companyId));
  await expect(page.getByLabel(/type/i)).toHaveText(newData.type.name);
});

test("should see not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.other.update] });
  await page.goto(routes.companies.edit(1));

  await expectMinimalUi(page, undefined, {
    companyFields: false,
    submitButton: false,
    title: "minimal",
    notFound: true
  });
});

for (const claim of [
  claims.company.any.manager.setFromNoneToSelf,
  claims.company.any.manager.setFromNoneToAny,
  claims.company.any.manager.setFromAnyToSelf,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from none to self in any company with claim ${claim}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claim]
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
  claims.company.any.manager.setFromNoneToAny,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from none to any in any company with claim ${claim}`, async ({
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
  claims.company.any.manager.setFromSelfToAny,
  claims.company.any.manager.setFromSelfToNone,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from self to none in any company with claim ${claim}`, async ({
    page,
    runAsDefaultUser,
    createCompany,
    getCompany
  }) => {
    const user = await runAsDefaultUser({
      claims: [claim]
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
  claims.company.any.manager.setFromAnyToNone,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from any to none in any company with claim ${claim}`, async ({
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

for (const claim of [claims.company.any.manager.setFromAnyToAny]) {
  test(`should be able to set manager from any to any in any company with claim ${claim}`, async ({
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
  claims.company.any.manager.setFromSelfToAny,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from self to any in any company with claim ${claim}`, async ({
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
  claims.company.any.manager.setFromAnyToSelf,
  claims.company.any.manager.setFromAnyToAny
]) {
  test(`should be able to set manager from any to self in any company with claim ${claim}`, async ({
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

async function checkAndFillFields(
  page: Page,
  company: Company
): Promise<Company> {
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
  await type.selectOption(newCompany.typeId?.toString() ?? null);
  const typeName =
    (await type.getByRole("option", { selected: true }).textContent()) ?? "";
  return {
    address: newCompany.address,
    ceo: newCompany.ceo,
    contacts: newCompany.contacts,
    email: newCompany.email,
    id: newCompany.id,
    inn: newCompany.inn,
    name: newCompany.name,
    phone: newCompany.phone,
    type: {
      id: newCompany.typeId ?? 0,
      name: typeName
    }
  };
}