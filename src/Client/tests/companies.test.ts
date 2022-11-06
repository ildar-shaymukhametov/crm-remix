import { expect } from '@playwright/test';
import { test } from './test';
import { getRemote } from 'mockttp'
import { buildCompany } from 'mocks/company';
const server = getRemote();

test.beforeEach(async ({ login }) => {
  await server.start(Number(process.env.API_URL_PORT));
  await server.forGet("/companies").thenReply(200, JSON.stringify([]));
  await login()
})

test.afterEach(async () => {
  await server.stop()
})

test("page has title 'Companies'", async ({ page }) => {
  await page.goto("/companies");
  await expect(page).toHaveTitle(/companies/i);
});

test("has companies", async ({ page }) => {
  const company = buildCompany();
  server.reset();
  await server.forGet("/companies").thenReply(200, JSON.stringify([company]));

  await page.goto("/companies")

  const listItem = page.getByText(company.name);
  await expect(listItem).toBeVisible();
  await server.stop();
});

test("page has 'new company' button", async ({ page }) => {
  await page.goto("/companies")

  const newCompanyButton = page.getByText(/new company/i);
  await expect(newCompanyButton).toBeVisible();

  await newCompanyButton.click();
  await expect(page).toHaveURL("/companies/new");
});

