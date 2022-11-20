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
