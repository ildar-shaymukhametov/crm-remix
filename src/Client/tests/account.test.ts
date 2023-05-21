import { expect } from "@playwright/test";
import { routes } from "~/utils/constants";
import { test } from "./account-test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("account", () => {
  test.describe("access", () => {
    test("should be able to save changes", async ({
      page,
      runAsDefaultUser,
      getClaimTypes
    }) => {
      await runAsDefaultUser();
      page.goto(routes.account.access);
      await expect(page.getByText(/forbidden/i)).not.toBeVisible();

      const claimTypes = await getClaimTypes();
      const elements = claimTypes.map(x => page.getByLabel(x.value));

      for (const item of elements) {
        await expect(item).toBeVisible();
        await expect(item).not.toBeChecked();
        await item.check();
      }

      const saveButton = page.getByRole("button", { name: /save/i });
      await expect(saveButton).toBeVisible();
      await saveButton.click();

      await page.reload();

      for (const item of elements) {
        await expect(item).toBeChecked();
      }
    });
  });
});
