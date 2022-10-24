import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useLoaderData } from "@remix-run/react";
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
    return null;
  }

  const data = await response.json();
  return json({ companies: data });
};

export default function CompanyIndex() {
  const data = useLoaderData<LoaderData>();
  if (!data) {
    return <div>Failed to load companies</div>;
  }

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
