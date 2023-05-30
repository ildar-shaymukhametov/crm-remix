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

// Edit company button

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

// Delete company button

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
