import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { test } from "./test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("account", () => {
  test.describe("access", () => {
    test("saves changes", async ({
      page,
      runAsDefaultUser
    }) => {
      await runAsDefaultUser();
      page.goto(routes.account.access);
      await expect(page.getByText(/forbidden/i)).not.toBeVisible();

      const claims = [
        "Company.Create",
        "Company.New.Other.Set",
        "Company.New.Manager.SetToAny",
        "Company.New.Manager.SetToSelf",
        "Company.New.Manager.SetToNone",
        "Company.Any.Delete",
        "Company.Any.Manager.Get",
        "Company.Any.Manager.SetFromAnyToAny",
        "Company.Any.Manager.SetFromAnyToSelf",
        "Company.Any.Manager.SetFromNoneToAny",
        "Company.Any.Manager.SetFromNoneToSelf",
        "Company.Any.Manager.SetFromAnyToNone",
        "Company.Any.Manager.SetFromSelfToAny",
        "Company.Any.Manager.SetFromSelfToNone",
        "Company.Any.Other.Get",
        "Company.Any.Other.Set",
        "Company.Any.Name.Get",
        "Company.Any.Name.Set",
        "Company.WhereUserIsManager.Delete",
        "Company.WhereUserIsManager.Other.Get",
        "Company.WhereUserIsManager.Other.Set",
        "Company.WhereUserIsManager.Manager.Get",
        "Company.WhereUserIsManager.Manager.SetFromSelfToAny",
        "Company.WhereUserIsManager.Manager.SetFromSelfToNone",
        "Company.WhereUserIsManager.Name.Get",
        "Company.WhereUserIsManager.Name.Set"
      ];

      for (const claim of claims) {
        const element = page.getByLabel(claim);
        await expect(element).toBeVisible();
        await expect(element).not.toBeChecked();
        await element.check();
      }

      const saveButton = page.getByRole("button", { name: /save/i });
      await expect(saveButton).toBeVisible();
      await saveButton.click();

      await page.reload();

      for (const claim of claims) {
        await expect(page.getByLabel(claim)).toBeChecked();
      }
    });
  });
});
