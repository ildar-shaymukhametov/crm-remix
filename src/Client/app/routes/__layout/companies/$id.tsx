import type { LoaderFunction, MetaFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import { permissions } from "~/utils/constants";

type LoaderData = {
  company: Company;
  userPermissions: string[];
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, {
    permissions: [
      permissions.viewCompany,
      permissions.updateCompany,
      permissions.deleteCompany
    ]
  });
  if (!user.permissions.includes(permissions.viewCompany)) {
    throw new Response(null, { status: 403 });
  }

  invariant(params.id, "Missing id parameter");
  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );
  return json({ company, userPermissions: user.permissions });
};

export default function CompanyRoute() {
  const { company, userPermissions } = useLoaderData<LoaderData>();

  return (
    <>
      {userPermissions.includes(permissions.updateCompany) ? (
        <Link to="edit">Edit</Link>
      ) : null}
      {userPermissions.includes(permissions.deleteCompany) ? (
        <Link to="delete">Delete</Link>
      ) : null}
      <div>
        {Object.entries(company).map(([key, name], i) => (
          <p key={i}>
            <span aria-label={key}>{key}</span>: {name}
          </p>
        ))}
      </div>
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

export const meta: MetaFunction<LoaderData> = ({ data }) => {
  if (!data?.company) {
    return {
      title: "View company"
    };
  }

  return {
    title: data.company.name
  };
};
