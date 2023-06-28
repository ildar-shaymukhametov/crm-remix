import { handleErrorResponse } from "./utils";

export type NewCompany = {
  id: number;
  typeId?: number;
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

export type Company = {
  id: number;
  fields: { [key: string]: object };
  canBeDeleted: boolean;
  canBeUpdated: boolean;
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

export async function updateCompany(
  request: Request,
  id: string,
  data: object,
  accessToken: string
): Promise<{ errors: string[][] } | undefined> {
  const response = await fetch(`${process.env.API_URL}/companies/${id}`, {
    method: "put",
    body: JSON.stringify({ ...data }),
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

export type NewCompanyVm = {
  fields: { [key: string]: object };
  initData: NewCompanyInitData;
};

type NewCompanyInitData = {
  managers: Manager[];
  companyTypes: CompanyType[];
};

export type Manager = {
  id: string;
  firstName?: string;
  lastName?: number;
};

export type CompanyType = {
  id: number;
  name: string;
};

export async function getNewCompany(
  request: Request,
  accessToken: string
): Promise<NewCompanyVm> {
  const response = await fetch(`${process.env.API_URL}/companies/new`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  return await response.json();
}

export type UpdateCompanyQuery = {
  id: number;
  fields: { [key: string]: object };
  initData: UpdateCompanyInitData;
};

type UpdateCompanyInitData = {
  managers: Manager[];
  companyTypes: CompanyType[];
};

export async function getUpdateCompanyData(
  request: Request,
  id: string,
  accessToken: string
): Promise<UpdateCompanyQuery> {
  const response = await fetch(
    `${process.env.API_URL}/companies/${id}/update`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    }
  );

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  return await response.json();
}
