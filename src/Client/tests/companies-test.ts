import { getAdminAccessToken, test as base } from "./test";
import { faker } from "@faker-js/faker";
import type { Company, NewCompany } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
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

        const result = await getCompany(
          new Request("http://foobar.com"),
          id.toString(),
          accessToken
        );

        return result;
      });
    },
    { auto: true }
  ]
});

export function buildCompany(options: CreateCompanyOptions = {}): NewCompany {
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
