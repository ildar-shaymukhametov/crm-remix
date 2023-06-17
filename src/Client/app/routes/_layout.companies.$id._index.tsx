import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import { isRouteErrorResponse, useRouteError } from "@remix-run/react";
import { Link, useLoaderData, useParams } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";

type LoaderData = {
  company: Company;
  userPermissions: {
    canUpdateCompany: boolean;
    canDeleteCompany: boolean;
  };
};

export const loader: LoaderFunction = async ({ request, params }) => {
  invariant(params.id, "Missing id parameter");

  const user = await auth.requireUser(request);
  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );

  return json({
    company
  });
};

export default function CompanyRoute() {
  const { company } = useLoaderData<LoaderData>();
  const { id } = useParams();

  return (
    <>
      {company.canBeUpdated ? (
        <Link to={routes.companies.edit(id)}>Edit</Link>
      ) : null}
      {company.canBeDeleted ? (
        <Link to={routes.companies.delete(id)}>Delete</Link>
      ) : null}
      <div>
        {"Name" in company.fields ? (
          <p>
            <span>name</span>:{" "}
            <span aria-label="name">{company.fields.Name?.toString()}</span>
          </p>
        ) : null}
        {"Type" in company.fields ? (
          <p>
            <span>type</span>:{" "}
            <span aria-label="type">
              {(company.fields.Type as CompanyType)?.name}
            </span>
          </p>
        ) : null}
        {"Inn" in company.fields ? (
          <p>
            <span>inn</span>:{" "}
            <span aria-label="inn">{company.fields.Inn?.toString()}</span>
          </p>
        ) : null}
        {"Address" in company.fields ? (
          <p>
            <span>address</span>:{" "}
            <span aria-label="address">
              {company.fields.Address?.toString()}
            </span>
          </p>
        ) : null}
        {"Ceo" in company.fields ? (
          <p>
            <span>ceo</span>:{" "}
            <span aria-label="ceo">{company.fields.Ceo?.toString()}</span>
          </p>
        ) : null}
        {"Phone" in company.fields ? (
          <p>
            <span>phone</span>:{" "}
            <span aria-label="phone">{company.fields.Phone?.toString()}</span>
          </p>
        ) : null}
        {"Email" in company.fields ? (
          <p>
            <span>email</span>:{" "}
            <span aria-label="email">{company.fields.Email?.toString()}</span>
          </p>
        ) : null}
        {"Contacts" in company.fields ? (
          <p>
            <span>contacts</span>:{" "}
            <span aria-label="contacts">
              {company.fields.Contacts?.toString()}
            </span>
          </p>
        ) : null}
        {"Manager" in company.fields ? (
          <p>
            <span>manager</span>:{" "}
            <span aria-label="manager">
              {company.fields.Manager
                ? `${(company.fields.Manager as Manager)?.lastName} ${
                    (company.fields.Manager as Manager)?.firstName
                  }`
                : "-"}
            </span>
          </p>
        ) : null}
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
  if (data?.company?.fields?.Name != undefined) {
    return [
      {
        title: data.company.fields.Name
      }
    ];
  }

  return [
    {
      title: "View company"
    }
  ];
};
