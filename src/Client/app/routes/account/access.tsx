import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useCatch, useLoaderData } from "@remix-run/react";
import { updateAuthorizationClaims } from "~/utils/account.server";
import { auth } from "~/utils/auth.server";

type ClaimType = {
  id: number;
  name: string;
  value: string;
};

type LoaderData = {
  claimTypes: ClaimType[];
  claims: string[];
};

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const formData = await request.formData();
  const data = {
    claims: Object.values(Object.fromEntries(formData)) as string[]
  };

  await updateAuthorizationClaims(data, user.extra?.access_token);
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(`${process.env.API_URL}/UserClaimTypes`, {
    headers: {
      Authorization: `Bearer ${user.extra?.access_token}`
    }
  });
  const userClaimsResponse = await fetch(
    `${process.env.API_URL}/User/AuthorizationClaims`,
    {
      headers: {
        Authorization: `Bearer ${user.extra?.access_token}`
      }
    }
  );

  if (!response.ok) {
    throw response;
  }

  const claimTypes = await response.json();
  const claims = await userClaimsResponse.json();
  return json({ claimTypes, claims });
};

export default function AccessRoute() {
  const { claimTypes, claims } = useLoaderData<LoaderData>();

  return (
    <form method="post">
      {claimTypes.map((x, i) => (
        <p key={x.id}>
          <label>
            {x.name}
            <input
              type="checkbox"
              name={`claims[${i}]`}
              value={x.value}
              defaultChecked={claims.includes(x.value)}
            />
          </label>
        </p>
      ))}
      <button type="submit">Save</button>
    </form>
  );
}

export function CatchBoundary() {
  const res = useCatch();
  if (res.status === 401) {
    return <p>Unauthorized</p>;
  }
  if (res.status === 403) {
    return <p>Forbidden</p>;
  }
  if (res.status === 404) {
    return <p>Company not found</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${res.status}`);
}

export function ErrorBoundary({ error }: { error: Error }) {
  console.error(error.message);
  return <p>Unexpected error</p>;
}
