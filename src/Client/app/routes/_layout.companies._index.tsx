import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import {
  Link,
  isRouteErrorResponse,
  useLoaderData,
  useRouteError
} from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";
import { getCompanies } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

type LoaderData = {
  companies: Company[];
  userPermissions: {
    canCreateCompany: boolean;
  };
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.company.create]
  });

  return json({
    companies: await getCompanies(request, user.extra.access_token),
    userPermissions: {
      canCreateCompany: user.permissions.includes(permissions.company.create)
    }
  });
};

export default function CompaniesIndexRoute() {
  const { companies, userPermissions } = useLoaderData<LoaderData>();

  return (
    <>
      {userPermissions.canCreateCompany ? (
        <Link to={routes.companies.new}>New company</Link>
      ) : null}
      <ul>
        {companies.length > 0 ? (
          companies.map((x, i) => (
            <li key={i}>
              {"Name" in x.fields ? (
                <span aria-label="name">{x.fields.Name?.toString()}</span>
              ) : null}
              {"Address" in x.fields ? (
                <span aria-label="address">{x.fields.Address?.toString()}</span>
              ) : null}
              {"Ceo" in x.fields ? (
                <span aria-label="ceo">{x.fields.Ceo?.toString()}</span>
              ) : null}
              {"Contacts" in x.fields ? (
                <span aria-label="contacts">
                  {x.fields.Contacts?.toString()}
                </span>
              ) : null}
              {"Email" in x.fields ? (
                <span aria-label="email">{x.fields.Email?.toString()}</span>
              ) : null}
              {"Inn" in x.fields ? (
                <span aria-label="inn">{x.fields.Inn?.toString()}</span>
              ) : null}
              {"Phone" in x.fields ? (
                <span aria-label="phone">{x.fields.Phone?.toString()}</span>
              ) : null}
              {"Type" in x.fields ? (
                <span aria-label="type">
                  {(x.fields.Type as CompanyType)?.name?.toString()}
                </span>
              ) : null}
              {"Manager" in x.fields ? (
                <p>
                  <span>manager</span>:{" "}
                  <span aria-label="manager">
                    {x.fields.Manager
                      ? `${(x.fields.Manager as Manager)?.lastName} ${
                          (x.fields.Manager as Manager)?.firstName
                        }`
                      : "-"}
                  </span>
                </p>
              ) : null}
              <Link to={routes.companies.view(x.id)}>""</Link>
              {x.canBeUpdated ? (
                <Link
                  to={routes.companies.edit(x.id)}
                  aria-label="edit company"
                >
                  ✏
                </Link>
              ) : null}
              {x.canBeDeleted ? (
                <Link
                  to={routes.companies.delete(x.id)}
                  aria-label="delete company"
                >
                  🗑
                </Link>
              ) : null}
            </li>
          ))
        ) : (
          <div>No companies found</div>
        )}
      </ul>
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

export function meta() {
  return [
    {
      title: "Companies"
    }
  ];
}
