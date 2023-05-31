import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { buildCompany, test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

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
    claims: [claims.company.create, claims.company.any.other.view]
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
  claims.company.any.manager.setFromAnyToAny,
  claims.company.any.manager.setFromAnyToSelf,
  claims.company.any.manager.setFromNoneToSelf,
  claims.company.any.manager.setFromNoneToAny
]) {
  test(`should be able to set manager to self with claim ${claim}`, async ({
    page,
    runAsDefaultUser
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.create, claims.company.any.other.view, claim]
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
  claims.company.any.manager.setFromAnyToAny,
  claims.company.any.manager.setFromNoneToAny
]) {
  test(`should be able to set manager to any with claim ${claim}`, async ({
    page,
    runAsDefaultUser,
    createUser
  }) => {
    await runAsDefaultUser({
      claims: [claims.company.create, claims.company.any.other.view, claim]
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
