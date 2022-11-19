import { expect } from "@playwright/test";
import { test } from "./test";

test.beforeEach(async ({ resetDb }) => {
  await resetDb();
});

test.describe("account", () => {
  test.describe("access", () => {
    test("should be able to save changes", async ({
      page,
      runAsDefaultUser,
    }) => {
      await runAsDefaultUser();
      page.goto("/account/access");
      await expect(page.getByText(/forbidden/i)).not.toBeVisible();

      const elements = [
        "Company. Create",
        "Company. Update",
        "Company. Delete",
        "Company. View",
      ].map((x) => page.getByLabel(x));

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
