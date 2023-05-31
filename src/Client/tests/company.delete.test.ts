import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
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
  const companyId = await createCompany();
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
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
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
    title: "minimal"
  });
});

test("should be forbidden if non-owned company but with claim", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.delete]
  });
  const companyId = await createCompany();
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
    title: "minimal"
  });
});

test("should be able to click delete company in any company", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.any.delete]
  });
  const companyId = await createCompany();
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company);

  const deleteButton = page.getByRole("button", {
    name: /delete company/i
  });
  await deleteButton.click();

  await expect(page).toHaveURL(routes.companies.index);
});

test("should be able to click delete own company with claim", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.delete]
  });

  const companyId = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company);

  const deleteButton = page.getByRole("button", {
    name: /delete company/i
  });
  await deleteButton.click();

  await expect(page).toHaveURL(routes.companies.index);
});

test("should be able to cancel", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.any.delete]
  });
  const companyId = await createCompany();
  await page.goto(routes.companies.delete(companyId));

  const company = await getCompany(companyId);
  await expectMinimalUi(page, company);

  const cancel = page.getByRole("link", { name: /cancel/i });
  await cancel.click();

  await expect(page).toHaveURL(routes.companies.view(companyId));
});

test("should see not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.delete] });
  await page.goto(routes.companies.delete(1));

  await expectMinimalUi(page, undefined, {
    cancelButton: false,
    okButton: false,
    title: "minimal",
    notFound: true
  });
});

type VisibilityOptions = {
  forbidden?: boolean;
  okButton?: boolean;
  cancelButton?: boolean;
  title?: "minimal" | "full";
  notFound?: boolean;
};

async function expectMinimalUi(
  page: Page,
  company?: Company,
  {
    forbidden = false,
    okButton = true,
    cancelButton = true,
    title = "full",
    notFound = false
  }: VisibilityOptions = {}
) {
  if (title === "minimal") {
    await expect(page).toHaveTitle("Delete company");
  } else {
    if (company) {
      await expect(page).toHaveTitle(`${company.name} • Delete`);
    }
  }
  await expect(page.getByText(/forbidden/i)).toBeVisible({
    visible: forbidden
  });

  const deleteButtonElem = page.getByRole("button", {
    name: /delete company/i
  });
  await expect(deleteButtonElem).toBeVisible({
    visible: okButton
  });

  const cancelButtonElem = page.getByRole("link", { name: /cancel/i });
  await expect(cancelButtonElem).toBeVisible({
    visible: cancelButton
  });

  await expect(page.getByText(/company not found/i)).toBeVisible({
    visible: notFound
  });
}
