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
import { PencilIcon, TrashIcon } from "@heroicons/react/24/outline";

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
      <table>
        <thead>
          <th>Name</th>
          {companies.some(x => "address" in x.fields) ? <th>Address</th> : null}
          {companies.some(x => "ceo" in x.fields) ? <th>Ceo</th> : null}
          {companies.some(x => "contacts" in x.fields) ? (
            <th>Contacts</th>
          ) : null}
          {companies.some(x => "email" in x.fields) ? <th>Email</th> : null}
          {companies.some(x => "inn" in x.fields) ? <th>Inn</th> : null}
          {companies.some(x => "phone" in x.fields) ? <th>Phone</th> : null}
          {companies.some(x => "type" in x.fields) ? <th>Type</th> : null}
          {companies.some(x => "manager" in x.fields) ? <th>Manager</th> : null}
          <th></th>
        </thead>
        <tbody>
          {companies.length > 0 ? (
            companies.map((x, i) => (
              <tr key={i}>
                <td>
                  <Link to={routes.companies.view(x.id)}>
                    {"name" in x.fields
                      ? x.fields.name?.toString()
                      : "<forbidden to see the name>"}
                  </Link>
                </td>
                <td>
                  {"address" in x.fields ? (
                    <span>{x.fields.address?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"ceo" in x.fields ? (
                    <span>{x.fields.ceo?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"contacts" in x.fields ? (
                    <span>{x.fields.contacts?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"email" in x.fields ? (
                    <span>{x.fields.email?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"inn" in x.fields ? (
                    <span>{x.fields.inn?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"phone" in x.fields ? (
                    <span>{x.fields.phone?.toString()}</span>
                  ) : null}
                </td>
                <td>
                  {"type" in x.fields ? (
                    <span>
                      {(x.fields.type as CompanyType)?.name?.toString()}
                    </span>
                  ) : null}
                </td>
                <td>
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
                </td>
                <td className="flex">
                  {x.canBeUpdated ? (
                    <Link
                      to={routes.companies.edit(x.id)}
                      aria-label="edit company"
                    >
                      <PencilIcon className="h-5 w-5" />
                    </Link>
                  ) : null}
                  {x.canBeDeleted ? (
                    <Link
                      to={routes.companies.delete(x.id)}
                      aria-label="delete company"
                    >
                      <TrashIcon className="h-5 w-5" />
                    </Link>
                  ) : null}
                </td>
              </tr>
            ))
          ) : (
            <div>No companies found</div>
          )}
        </tbody>
      </table>
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
