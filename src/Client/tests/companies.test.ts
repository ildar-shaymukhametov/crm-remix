import { expect } from '@playwright/test';
import { test } from './test';

test.beforeEach(async ({ login }) => {
  await login()
})

test("page has title 'Companies'", async ({ page, login }) => {
  await page.goto("/companies");
  await expect(page).toHaveTitle("Companies");
});

test("page has 'new company' button", async ({ page }) => {
  await page.goto("/companies")

  const newCompanyButton = page.getByText("New company");
  await expect(newCompanyButton).toBeVisible();

  await newCompanyButton.click();
  await expect(page).toHaveURL("/companies/new");
});

