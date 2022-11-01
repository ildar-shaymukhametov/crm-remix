import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import type { Company } from "~/models/company";
import { auth } from "~/utils/auth.server";
import type { OidcProfile } from "~/utils/oidc-strategy";

type LoaderData = {
  company: Company;
};

export function getCompanyRouteNavbarButtons(id: string, user: OidcProfile) {
  let result = [];
  if (user.authrizationClaims.includes("company.delete")) {
    result.push(<Link to={`companies/${id}/delete`}>Delete</Link>);
  }
  if (user.authrizationClaims.includes("company.update")) {
    result.push(<Link to={`companies/${id}/edit`}>Edit</Link>);
  }

  return result;
}

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(
    `${process.env.API_URL}/companies/${params.id}`,
    {
      headers: {
        Authorization: `Bearer ${user.extra?.accessToken}`,
      },
    }
  );

  if (!response.ok) {
    throw response;
  }

  return json({ company: await response.json() });
};

export default function CompanyRoute() {
  const data = useLoaderData<LoaderData>();

  return Object.entries(data.company).map(([key, name], i) => (
    <p key={i}>
      {key}: {name}
    </p>
  ));
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
