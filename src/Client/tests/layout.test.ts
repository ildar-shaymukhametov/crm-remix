import { expect } from "@playwright/test";
import { test } from "./test";

test("navbar", async ({ page, runAsDefaultUser }) => {
  const user = await runAsDefaultUser();
  await page.goto("/");
  await expect(page.getByRole("button", { name: /log out/i })).toBeVisible();
  await expect(page.getByText(user.userName)).toBeVisible();
});
