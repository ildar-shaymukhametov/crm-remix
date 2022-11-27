import { handleErrorResponse } from "./utils";

export async function createCompany(
  request: Request,
  data: { [key: string]: any },
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
  type: string;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
};

export async function getCompany(request: Request,
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

export async function deleteCompany(request: Request,
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

export async function updateCompany(request: Request,
  id: string,
  data: { [key: string]: any },
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

export async function getCompanies(request: Request, accessToken: string): Promise<Company[]> {
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
