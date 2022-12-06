import type { LoaderFunction, MetaFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData, useParams } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

type LoaderData = {
  company: Company;
  userPermissions: {
    canUpdateCompany: boolean;
    canDeleteCompany: boolean;
  };
};

export const loader: LoaderFunction = async ({ request, params }) => {
  invariant(params.id, "Missing id parameter");

  const user = await auth.requireUser(request, {
    key: params.id,
    permissions: [
      permissions.viewCompany,
      permissions.updateCompany,
      permissions.deleteCompany
    ]
  });
  if (!user.permissions.includes(permissions.viewCompany)) {
    throw new Response(null, { status: 403 });
  }

  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );
  return json({ company, userPermissions: {
    canUpdateCompany: user.permissions.includes(permissions.updateCompany),
    canDeleteCompany: user.permissions.includes(permissions.deleteCompany)
  } });
};

export default function CompanyRoute() {
  const { company, userPermissions } = useLoaderData<LoaderData>();
  const { id } = useParams();

  return (
    <>
      {userPermissions.canUpdateCompany ? (
        <Link to={routes.companies.edit(id)}>Edit</Link>
      ) : null}
      {userPermissions.canDeleteCompany ? (
        <Link to={routes.companies.delete(id)}>Delete</Link>
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
