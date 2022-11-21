export async function createCompany(
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
    throw response;
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

export async function getCompany(
  id: string,
  accessToken: string
): Promise<Company> {
  const response = await fetch(`${process.env.API_URL}/companies/${id}`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    throw response;
  }

  const company = await response.json();
  return company;
}

export async function deleteCompany(
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
    throw response;
  }
}

export async function updateCompany(
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
    throw response;
  }

  const { errors } = await response.json();
  return errors;
}

export async function getCompanies(accessToken: string): Promise<Company[]> {
  const response = await fetch(`${process.env.API_URL}/companies`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    throw response;
  }

  const companies = await response.json();
  return companies;
}
