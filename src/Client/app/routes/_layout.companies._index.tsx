import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import {
  Link,
  isRouteErrorResponse,
  useLoaderData,
  useRouteError
} from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { CompanyIndex } from "~/utils/companies.server";
import { getCompanies } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

type LoaderData = {
  companies: CompanyIndex[];
  userPermissions: {
    canCreateCompany: boolean;
  };
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.company.create]
  });

  const companies = await getCompanies(request, user.extra.access_token);
  return json({
    companies,
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
              <Link to={routes.companies.view(x.id)}>{x.name}</Link>
              {x.canBeEdited ? (
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
