import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("minimal ui", async ({ page, runAsDefaultUser, createCompany }) => {
  await runAsDefaultUser();
  await createCompany();
  await page.goto(routes.companies.index);

  await expectMinimalUi(page, { noCompaniesFound: true });
});

test("clicks new company button", async ({
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

test("does not see own company without claim", async ({
  page,
  runAsDefaultUser,
  createCompany
}) => {
  const user = await runAsDefaultUser();

  await createCompany({ managerId: user.id });
  await page.goto(routes.companies.index);

  await expectMinimalUi(page, { noCompaniesFound: true });
});

test.describe("edit button", () => {
  for (const claim of [
    claims.company.any.other.set,
    claims.company.any.name.set,
    claims.company.any.manager.setFromAnyToAny,
    claims.company.any.manager.setFromAnyToNone,
    claims.company.any.manager.setFromAnyToSelf,
    claims.company.any.manager.setFromNoneToAny,
    claims.company.any.manager.setFromNoneToSelf
  ]) {
    test(`clicks in non-owned company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany
    }) => {
      await runAsDefaultUser({
        claims: [claim]
      });

      const companyId = await createCompany();
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, { editCompanyButton: true });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(companyId));
    });
  }

  for (const claim of [
    claims.company.any.other.set,
    claims.company.whereUserIsManager.other.set,
    claims.company.any.name.set,
    claims.company.whereUserIsManager.name.set,
    claims.company.any.manager.setFromSelfToAny,
    claims.company.any.manager.setFromSelfToNone,
    claims.company.whereUserIsManager.manager.setFromSelfToAny,
    claims.company.whereUserIsManager.manager.setFromSelfToNone
  ]) {
    test(`clicks in own company with claim ${claim}`, async ({
      page,
      runAsDefaultUser,
      createCompany
    }) => {
      const user = await runAsDefaultUser({
        claims: [claim]
      });

      const companyId = await createCompany({ managerId: user.id });
      await page.goto(routes.companies.index);

      await expectMinimalUi(page, { editCompanyButton: true });

      const link = page.getByRole("link", { name: /edit company/i });
      await link.click();
      await expect(page).toHaveURL(routes.companies.edit(companyId));
    });
  }
});

test.describe("delete button", () => {
  test("clicks in own company", async ({
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

  test("clicks in non-owned company", async ({
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
