import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { Company } from "~/utils/companies.server";
import { getCompanies } from "~/utils/companies.server";
import { permissions } from "~/utils/constants";

type LoaderData = {
  companies: Company[];
  userPermissions: string[];
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.createCompany]
  });

  const companies = await getCompanies(request, user.extra.access_token);
  return json({ companies, userPermissions: user.permissions });
};

export default function CompanyIndex() {
  const { companies, userPermissions } = useLoaderData<LoaderData>();

  return (
    <>
      {userPermissions.includes(permissions.createCompany) ? (
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
