import { handleErrorResponse } from "./utils";

export type NewCompany = {
  id: number;
  typeId?: string;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
  managerId?: string;
};

export async function createCompany(
  request: Request,
  data: NewCompany,
  accessToken: string
): Promise<number> {
  const response = await fetch(`${process.env.API_URL}/companies`, {
    method: "post",
    body: JSON.stringify(data),
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "application/json"
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const { id } = await response.json();
  return id as number;
}

export async function createTestCompany(
  request: Request,
  data: NewCompany
): Promise<number> {
  const response = await fetch(`${process.env.API_URL}/test/companies`, {
    method: "post",
    body: JSON.stringify(data),
    headers: {
      "X-API-Key": "TestApiKey",
      "Content-Type": "application/json"
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const { id } = await response.json();
  return id as number;
}

export type Company = {
  id: number;
  type: CompanyType;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
  manager?: Manager;
};

export async function getCompany(
  request: Request,
  id: string,
  accessToken: string
): Promise<Company> {
  const response = await fetch(`${process.env.API_URL}/companies/${id}`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const company = await response.json();
  return company;
}

export async function getTestCompany(
  request: Request,
  id: string
): Promise<Company> {
  const response = await fetch(`${process.env.API_URL}/test/companies/${id}`, {
    headers: {
      "X-API-Key": "TestApiKey"
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const company = await response.json();
  return company;
}

export async function deleteCompany(
  request: Request,
  id: string,
  accessToken: string
): Promise<void> {
  const response = await fetch(`${process.env.API_URL}/companies/${id}`, {
    method: "delete",
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }
}

export type UpdateCompany = {
  typeId?: string;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
  managerId?: string;
};

export async function updateCompany(
  request: Request,
  id: string,
  data: UpdateCompany,
  accessToken: string
): Promise<{ errors: string[][] } | undefined> {
  const response = await fetch(`${process.env.API_URL}/companies/${id}`, {
    method: "put",
    body: JSON.stringify(data),
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-Type": "application/json"
    }
  });

  if (response.ok) {
    return;
  }

  if (response.status !== 400) {
    if (!response.ok) {
      await handleErrorResponse(request, response);
    }
  }

  const { errors } = await response.json();
  return errors;
}

export type CompanyIndex = {
  id: number;
  name: string;
  canBeEdited: boolean;
  canBeDeleted: boolean;
};

export async function getCompanies(
  request: Request,
  accessToken: string
): Promise<CompanyIndex[]> {
  const response = await fetch(`${process.env.API_URL}/companies`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const companies = await response.json();
  return companies;
}

type CompanyInitData = {
  managers: Manager[],
  companyTypes: CompanyType[],
};

export type Manager = {
  id: string,
  firstName?: string,
  lastName?: number
}

export type CompanyType = {
  id: number;
  name: string;
};

export async function getInitData(
  request: Request,
  accessToken: string,
  companyId?: number
): Promise<CompanyInitData> {
  const response = await fetch(`${process.env.API_URL}/companies/initData?id=${companyId}`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  return await response.json();
}
