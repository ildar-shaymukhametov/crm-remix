import { test as base } from "./test";
import { faker } from "@faker-js/faker";
import type { Company, NewCompany } from "~/utils/companies.server";
import { createTestCompany, getTestCompany } from "~/utils/companies.server";

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
        return await createTestCompany(
          new Request("http://foobar.com"),
          buildCompany(options)
        );
      });
    },
    { auto: true }
  ],
  getCompany: [
    async ({ page }, use) => {
      use(async (id) => {
        return await getTestCompany(
          new Request("http://foobar.com"),
          id.toString()
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
    type: faker.helpers.arrayElement(["ООО", "АО", "ПАО", "ИП"]),
    managerId: options.managerId
  };
}
