import { handleErrorResponse } from "./utils";

export async function updateAuthorizationClaims(
  request: Request,
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
    await handleErrorResponse(request, response);
  }
}

export async function getAuthorizationClaims(
  request: Request,
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
    await handleErrorResponse(request, response);
  }

  const claims = await response.json();
  return claims;
}

export type ClaimType = {
  id: number;
  name: string;
  value: string;
};

export async function getClaimTypes(
  request: Request,
  accessToken: string
): Promise<ClaimType[]> {
  const response = await fetch(`${process.env.API_URL}/UserClaimTypes`, {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    await handleErrorResponse(request, response);
  }

  const claims = await response.json();
  return claims;
}

