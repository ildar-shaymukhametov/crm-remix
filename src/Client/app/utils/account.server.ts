export async function updateAuthorizationClaims(
  data: { claims: string[] },
  accessToken: string
): Promise<void> {
  const response = await fetch(
    `${process.env.API_URL}/User/AuthorizationClaims`,
    {
      method: "post",
      body: JSON.stringify(data),
      headers: {
        Authorization: `Bearer ${accessToken}`,
        "Content-Type": "application/json"
      }
    }
  );

  if (!response.ok) {
    throw response;
  }
}
