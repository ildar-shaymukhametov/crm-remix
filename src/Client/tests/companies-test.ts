import { getAdminAccessToken, test as base } from "./test";
import { faker } from "@faker-js/faker";
import type { Company } from "~/utils/companies.server";
import { createCompany } from "~/utils/companies.server";

type CreateCompanyOptions = {
  managerId?: string
};

export const test = base.extend<{
  createCompany: (options?: CreateCompanyOptions) => Promise<Company>;
}>({
  createCompany: [
    async ({ page }, use) => {
      use(async (options = {}) => {
        const accessToken = await getAdminAccessToken(page);
        const company = buildCompany(options);
        const id = await createCompany(
          new Request("http://foobar.com"),
          company,
          accessToken
        );
        company.id = id;

        return company;
      });
    },
    { auto: true }
  ]
});

export function buildCompany(options: CreateCompanyOptions = {}): Company {
  return {
    address: faker.location.streetAddress(),
    ceo: faker.person.fullName(),
    contacts: faker.internet.email(),
    email: faker.internet.email(),
    id: faker.number.int(),
    inn: faker.string.numeric(10),
    name: faker.company.name(),
    phone: faker.phone.number(),
    type: faker.helpers.arrayElement(["ООО", "АО", "ПАО", "ИП"]),
    managerId: options.managerId
  };
}
