import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { buildCompany, test } from "./companies-test";
import type { NewCompany } from "~/utils/companies.server";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("forbidden", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser();
  await page.goto(routes.companies.new);

  await expectMinimalUi(page, {
    forbidden: true,
    submitButton: false,
    requiredFields: false
  });
});

test("creates company with required fields", async ({
  page,
  runAsDefaultUser
}) => {
  await runAsDefaultUser({
    claims: [claims.company.create]
  });
  await page.goto(routes.companies.new);
  await expectMinimalUi(page);

  const company = buildCompany();
  await fillRequiredFields(page, company);

  const submit = page.getByRole("button", { name: /create new company/i });
  await submit.click();

  await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
});

test.describe("manager", () => {
  test(`sets to self with claim ${claims.company.new.manager.setToAny}`, async ({
    page,
    runAsDefaultUser
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.create, claims.company.new.manager.setToAny]
    });

    await page.goto(routes.companies.new);
    await expectMinimalUi(page, { manager: true });

    await fillRequiredFields(page, buildCompany());

    const manager = page.getByLabel(/manager/i);
    await expect(manager.getByRole("option", { selected: true })).toHaveText(
      "-"
    );

    await manager.selectOption(user.id);

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();

    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });

  test(`sets to self with claim ${claims.company.new.manager.setToSelf}`, async ({
    page,
    runAsDefaultUser
  }) => {
    const user = await runAsDefaultUser({
      claims: [claims.company.create, claims.company.new.manager.setToSelf]
    });

    await page.goto(routes.companies.new);
    await expectMinimalUi(page, { manager: true });

    await fillRequiredFields(page, buildCompany());

    const manager = page.getByLabel(/manager/i);
    await expect(manager.getByRole("option", { selected: true })).toHaveText(
      `${user.lastName} ${user.firstName}`
    );

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();

    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });

  test("cannot set", async ({ page, runAsDefaultUser }) => {
    await runAsDefaultUser({
      claims: [claims.company.create, claims.company.new.manager.setToNone]
    });
    await page.goto(routes.companies.new);
    await expectMinimalUi(page);

    const company = buildCompany();
    await fillRequiredFields(page, company);

    const submit = page.getByRole("button", { name: /create new company/i });
    await submit.click();

    await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
  });
});

test("sets other fields", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({
    claims: [claims.company.create, claims.company.new.other.set]
  });

  await page.goto(routes.companies.new);
  await expectMinimalUi(page, { otherFields: true });

  await fillRequiredFields(page, buildCompany());

  const company = buildCompany();
  await page.getByLabel(/address/i).fill(company.address);
  await page.getByLabel(/ceo/i).fill(company.ceo);
  await page.getByLabel(/contacts/i).fill(company.contacts);
  await page.getByLabel(/email/i).fill(company.email);
  await page.getByLabel(/inn/i).fill(company.inn);
  await page.getByLabel(/phone/i).fill(company.phone);

  const type = page.getByLabel(/type/i);
  await type.selectOption({ index: 1 });

  const submit = page.getByRole("button", { name: /create new company/i });
  await submit.click();

  await expect(page).toHaveURL(new RegExp(`/companies/[\\d]+`));
});

type VisibilityOptions = {
  forbidden?: boolean;
  otherFields?: boolean;
  submitButton?: boolean;
  manager?: boolean;
  requiredFields?: boolean;
};

async function fillRequiredFields(page: Page, company: NewCompany) {
  await page.getByLabel(/name/i).fill(company.name);
}

async function expectMinimalUi(
  page: Page,
  {
    forbidden = false,
    otherFields = false,
    submitButton = true,
    manager = false,
    requiredFields = true
  }: VisibilityOptions = {}
) {
  await expect(page).toHaveTitle("New company");
  await expect(page.getByText(/forbidden/i)).toBeVisible({
    visible: forbidden
  });

  await expectFieldsToBeVisible(page, {
    name: requiredFields,
    address: otherFields,
    ceo: otherFields,
    contacts: otherFields,
    email: otherFields,
    inn: otherFields,
    phone: otherFields,
    type: otherFields,
    manager
  });

  const save = page.getByRole("button", { name: /create new company/i });
  await expect(save).toBeVisible({
    visible: submitButton
  });
}

async function expectFieldsToBeVisible(
  page: Page,
  data: { [key: string]: boolean }
) {
  for (const key of Object.keys(data)) {
    await expect(page.getByLabel(key)).toBeVisible({ visible: data[key] });
  }
}
