import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import {
  isRouteErrorResponse,
  useLoaderData,
  useRouteError
} from "@remix-run/react";
import { ButtonPrimary } from "~/components/buttons";
import type { ClaimType } from "~/utils/account.server";
import {
  getAuthorizationClaims,
  updateAuthorizationClaims,
  getClaimTypes
} from "~/utils/account.server";
import { auth } from "~/utils/auth.server";

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

  await updateAuthorizationClaims(request, data, user.extra?.access_token);
  return {};
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const claimTypes = await getClaimTypes(request, user.extra?.access_token);
  const claims = await getAuthorizationClaims(
    request,
    user.extra?.access_token
  );

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
      <ButtonPrimary type="submit">Save</ButtonPrimary>
    </form>
  );
}

export function ErrorBoundary() {
  const error = useRouteError();
  if (!isRouteErrorResponse(error)) {
    return <p>Unexpected error</p>;
  }

  if (error.status === 403) {
    return <p>Forbidden</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${error.status}`);
}
