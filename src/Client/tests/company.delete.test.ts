import type { Page } from "@playwright/test";
import { expect } from "@playwright/test";
import type { Company } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { claims } from "~/utils/constants.server";
import { test } from "./companies-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test("forbidden", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser();
  const id = await createCompany();
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
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
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
  });
});

test("forbidden if non-owned company but with claim", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.delete]
  });
  const id = await createCompany();
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, {
    cancelButton: false,
    forbidden: true,
    okButton: false,
  });
});

test("deletes any company", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.any.delete]
  });
  const id = await createCompany();
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company);

  const deleteButton = page.getByRole("button", {
    name: /delete company/i
  });
  await deleteButton.click();

  await expect(page).toHaveURL(routes.companies.index);
});

test("deletes own company", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  const user = await runAsDefaultUser({
    claims: [claims.company.whereUserIsManager.delete]
  });

  const id = await createCompany({ managerId: user.id });
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company);

  const deleteButton = page.getByRole("button", {
    name: /delete company/i
  });
  await deleteButton.click();

  await expect(page).toHaveURL(routes.companies.index);
});

test("cancels", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.any.delete]
  });
  const id = await createCompany();
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company);

  const cancel = page.getByRole("link", { name: /cancel/i });
  await cancel.click();

  await expect(page).toHaveURL(routes.companies.view(id));
});

test("not found", async ({ page, runAsDefaultUser }) => {
  await runAsDefaultUser({ claims: [claims.company.any.delete] });
  await page.goto(routes.companies.delete(1));

  await expectMinimalUi(page, undefined, {
    cancelButton: false,
    okButton: false,
    notFound: true
  });
});

test("title", async ({
  page,
  runAsDefaultUser,
  createCompany,
  getCompany
}) => {
  await runAsDefaultUser({
    claims: [claims.company.any.delete, claims.company.any.name.get]
  });
  const id = await createCompany();
  await page.goto(routes.companies.delete(id));

  const company = await getCompany(id);
  await expectMinimalUi(page, company, { title: "full" });

  const cancel = page.getByRole("link", { name: /cancel/i });
  await cancel.click();

  await expect(page).toHaveURL(routes.companies.view(id));
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
    title = "minimal",
    notFound = false
  }: VisibilityOptions = {}
) {
  if (title === "minimal") {
    await expect(page).toHaveTitle("Delete company");
  } else {
    if (company?.fields?.name != undefined) {
      await expect(page).toHaveTitle(company.fields.name.toString());
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
