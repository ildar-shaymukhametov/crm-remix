import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

type Company = {
  id: string;
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

export const handle = {
  navbarButtons: [<Link to="companies/1/delete">Delete</Link>]
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
    return json(null, { status: response.status });
  }

  return json({ company: await response.json() });
};

export default function CompanyRoute() {
  const data = useLoaderData<LoaderData>();

  if (!data?.company) {
    return <div>Failed to load company</div>;
  }

  return Object.entries(data.company).map(([key, name], i) => (
    <p key={i}>
      {key}: {name}
    </p>
  ));
}
