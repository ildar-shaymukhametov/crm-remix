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

export async function getAuthorizationClaims(
  accessToken: string
): Promise<string[]> {
  const response = await fetch(
    `${process.env.API_URL}/User/AuthorizationClaims`,
    {
      headers: {
        Authorization: `Bearer ${accessToken}`
      }
    }
  );

  if (!response.ok) {
    throw response;
  }

  const claims = await response.json();
  return claims;
}

export type ClaimType = {
  id: number;
  name: string;
  value: string;
};

export async function getClaimTypes(accessToken: string): Promise<ClaimType[]> {
  const response = await fetch(`${process.env.API_URL}/UserClaimTypes`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    throw response;
  }

  const claims = await response.json();
  return claims;
}
