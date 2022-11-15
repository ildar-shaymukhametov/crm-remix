import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

type Company = {
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
  company: Company;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, { permissions: ["ViewCompany"] });
  if (!user.permissions.includes("ViewCompany")) {
    throw new Response(null, { status: 403 });
  }

  const response = await fetch(
    `${process.env.API_URL}/companies/${params.id}`,
    {
      headers: {
        Authorization: `Bearer ${user.extra?.access_token}`,
      },
    }
  );

  if (!response.ok) {
    throw response;
  }

  return json({ company: await response.json() });
};

export default function CompanyRoute() {
  const { company } = useLoaderData<LoaderData>();

  return (
    <>
      <Link to={`companies/${company.id}/delete`}>Delete</Link>
      <Link to={`companies/${company.id}/edit`}>Edit</Link>
      <div>
        {Object.entries(company).map(([key, name], i) => (
          <p key={i}>
            {key}: {name}
          </p>
        ))}
      </div>
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
