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
  companies: Company[];
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(`${process.env.API_URL}/companies`, {
    headers: {
      Authorization: `Bearer ${user.extra?.accessToken}`,
    },
  });

  if (!response.ok) {
    throw response;
  }

  const data = await response.json();
  return json({ companies: data });
};

export default function CompanyIndex() {
  const data = useLoaderData<LoaderData>();

  return (
    <ul>
      {data.companies.map((x, i) => (
        <li key={i}>
          <Link to={x.id.toString()}>{x.name}</Link>
        </li>
      ))}
    </ul>
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
