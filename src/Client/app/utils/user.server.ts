import { handleErrorResponse } from "./utils";

export async function getUserPermissions(
  request: Request,
  requestedPermissions: string[],
  accessToken: string
): Promise<string[]> {
  const params: string[][] = [];
  requestedPermissions.forEach(x => params.push(["q", x]));
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
