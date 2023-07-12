import { test as base, getAdminAccessToken } from "./test";
import { faker } from "@faker-js/faker";
import type { Company, NewCompany } from "~/utils/companies.server";
import { createCompany, getCompany } from "~/utils/companies.server";

type CreateCompanyOptions = {
  managerId?: string
};

export const test = base.extend<{
  createCompany: (options?: CreateCompanyOptions) => Promise<number>;
  getCompany: (id: number) => Promise<Company>;
}>({
  createCompany: [
    async ({ page }, use) => {
      use(async (options = {}) => {
        return await createCompany(
          new Request("http://foobar.com"),
          buildCompany(options),
          await getAdminAccessToken(page)
        );
      });
    },
    { auto: true }
  ],
  getCompany: [
    async ({ page }, use) => {
      use(async (id) => {
        return await getCompany(
          new Request("http://foobar.com"),
          id.toString(),
          await getAdminAccessToken(page)
        );
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
    typeId: faker.helpers.arrayElement([1, 2, 3, 4]),
    managerId: options.managerId
  };
}
