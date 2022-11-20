import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { OidcProfile } from "~/utils/oidc-strategy";

export type Company = {
  id: number;
  type: string;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
};

type LoaderData = {
  companies: Company[];
  user: OidcProfile;
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: ["CreateCompany"],
  });
  const response = await fetch(`${process.env.API_URL}/companies`, {
    headers: {
      Authorization: `Bearer ${user.extra?.access_token}`,
    },
  });

  if (!response.ok) {
    throw response;
  }

  const data = await response.json();
  return json({ companies: data, user });
};

export default function CompanyIndex() {
  const { companies, user } = useLoaderData<LoaderData>();

  return (
    <>
      {user.permissions.includes("CreateCompany") ? (
        <Link to="/companies/new">New company</Link>
      ) : null}
      <ul>
        {companies.length > 0 ? (
          companies.map((x, i) => (
            <li key={i}>
              <Link to={x.id.toString()}>{x.name}</Link>
            </li>
          ))
        ) : (
          <div>No companies found</div>
        )}
      </ul>
    </>
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

export function meta() {
  return {
    title: "Companies"
  };
}
