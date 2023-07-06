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
              <span aria-label="name">
                {"name" in x.fields ? (
                  <Link to={routes.companies.view(x.id)}>
                    {x.fields.name?.toString()}
                  </Link>
                ) : (
                  "<forbidden to see the name>"
                )}
              </span>
              {"address" in x.fields ? (
                <span aria-label="address">{x.fields.address?.toString()}</span>
              ) : null}
              {"ceo" in x.fields ? (
                <span aria-label="ceo">{x.fields.ceo?.toString()}</span>
              ) : null}
              {"contacts" in x.fields ? (
                <span aria-label="contacts">
                  {x.fields.contacts?.toString()}
                </span>
              ) : null}
              {"email" in x.fields ? (
                <span aria-label="email">{x.fields.email?.toString()}</span>
              ) : null}
              {"inn" in x.fields ? (
                <span aria-label="inn">{x.fields.inn?.toString()}</span>
              ) : null}
              {"phone" in x.fields ? (
                <span aria-label="phone">{x.fields.phone?.toString()}</span>
              ) : null}
              {"type" in x.fields ? (
                <span aria-label="type">
                  {(x.fields.type as CompanyType)?.name?.toString()}
                </span>
              ) : null}
              {"manager" in x.fields ? (
                <p>
                  <span>manager</span>:{" "}
                  <span aria-label="manager">
                    {x.fields.manager
                      ? `${(x.fields.manager as Manager)?.lastName} ${
                          (x.fields.manager as Manager)?.firstName
                        }`
                      : "-"}
                  </span>
                </p>
              ) : null}
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
