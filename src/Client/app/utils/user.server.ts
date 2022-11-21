export async function getUserPermissions(
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
    throw response;
  }

  const { permissions } = await response.json();
  return permissions;
}
