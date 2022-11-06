import { faker } from '@faker-js/faker'
import type { Company } from '~/routes/__layout/companies/index'

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
  }
}