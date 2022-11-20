import { getAdminAccessToken, test as base } from "./test";
import type { Company } from "~/routes/__layout/companies/index";
import { faker } from "@faker-js/faker";
import { createCompany } from "~/utils/companies.server";

export const test = base.extend<{
  createCompany: () => Promise<Company>;
}>({
  createCompany: [
    async ({ page }, use) => {
      use(async () => {
        const accessToken = await getAdminAccessToken(page);
        const company = buildCompany();
        const id = await createCompany(company, accessToken);
        company.id = id;

        return company;
      });
    },
    { auto: true }
  ]
});

export function buildCompany(): Company {
  return {
    address: faker.address.streetAddress(),
    ceo: faker.name.fullName(),
    contacts: faker.internet.email(),
    email: faker.internet.email(),
    id: faker.datatype.number(),
    inn: faker.random.numeric(10),
    name: faker.company.name(),
    phone: faker.phone.number(),
    type: faker.helpers.arrayElement(["ООО", "АО", "ПАО", "ИП"])
  };
}
