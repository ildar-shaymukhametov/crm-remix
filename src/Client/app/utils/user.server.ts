import { handleErrorResponse } from "./utils";

export type ResourcePermissions = {
  key?: string;
  permissions: string[];
};

export async function getUserPermissions(
  request: Request,
  resource: ResourcePermissions,
  accessToken: string
): Promise<string[]> {
  const params: string[][] = [];
  resource.permissions.forEach(x => params.push(["q", x]));

  if (resource.key) {
    params.push(["resourceKey", resource.key]);
  }

  const query = new URLSearchParams(params);

  const response = await fetch(
    `${process.env.API_URL}/User/Permissions?${query}`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    }
  );

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const { permissions } = await response.json();
  return permissions;
}

export type NewUser = {
  userName: string;
  password: string;
  firstName?: string;
  lastName?: string;
  claims?: string[];
  roles?: string[];
};

export async function createUser(
  request: Request,
  data: NewUser
): Promise<string> {
  const response = await fetch(`${process.env.API_URL}/user`, {
    method: "post",
    body: JSON.stringify(data),
    headers: {
      "Content-Type": "application/json"
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const { id } = await response.json();
  return id as string;
}
