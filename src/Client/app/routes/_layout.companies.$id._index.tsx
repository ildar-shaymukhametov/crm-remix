import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import { isRouteErrorResponse, useRouteError } from "@remix-run/react";
import { Link, useLoaderData, useParams } from "@remix-run/react";
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
      permissions.company.view,
      permissions.company.update,
      permissions.company.delete
    ]
  });
  if (!user.permissions.includes(permissions.company.view)) {
    throw new Response(null, { status: 403 });
  }

  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );
  return json({
    company,
    userPermissions: {
      canUpdateCompany: user.permissions.includes(permissions.company.update),
      canDeleteCompany: user.permissions.includes(permissions.company.delete)
    }
  });
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
        <p>
          <span>name</span>: <span aria-label="name">{company.name}</span>
        </p>
        <p>
          <span>type</span>: <span aria-label="type">{company.type?.name}</span>
        </p>
        <p>
          <span>inn</span>: <span aria-label="inn">{company.inn}</span>
        </p>
        <p>
          <span>address</span>:{" "}
          <span aria-label="address">{company.address}</span>
        </p>
        <p>
          <span>ceo</span>: <span aria-label="ceo">{company.ceo}</span>
        </p>
        <p>
          <span>phone</span>: <span aria-label="phone">{company.phone}</span>
        </p>
        <p>
          <span>email</span>: <span aria-label="email">{company.email}</span>
        </p>
        <p>
          <span>contacts</span>:{" "}
          <span aria-label="contacts">{company.contacts}</span>
        </p>
        <p>
          <span>manager</span>:{" "}
          <span aria-label="manager">
            {company.manager
              ? `${company.manager?.firstName} ${company.manager?.lastName}`
              : "-"}
          </span>
        </p>
      </div>
    </>
  );
}

export function ErrorBoundary() {
  const error = useRouteError();
  if (!isRouteErrorResponse(error)) {
    return;
  }

  if (error.status === 403) {
    return <p>Forbidden</p>;
  }

  if (error.status === 404) {
    return <p>Company not found</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${error.status}`);
}

export const meta: V2_MetaFunction<typeof loader> = ({ data }) => {
  if (!data?.company) {
    return [
      {
        title: "View company"
      }
    ];
  }

  return [
    {
      title: data.company.name
    }
  ];
};
