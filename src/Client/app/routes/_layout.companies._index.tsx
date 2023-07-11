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

  const addressColVisible = companies.some(x => "address" in x.fields);
  const ceoColVisible = companies.some(x => "ceo" in x.fields);
  const contactsColVisible = companies.some(x => "contacts" in x.fields);
  const emailColVisible = companies.some(x => "email" in x.fields);
  const innColVisible = companies.some(x => "inn" in x.fields);
  const phoneColVisible = companies.some(x => "phone" in x.fields);
  const typeColVisible = companies.some(x => "type" in x.fields);
  const managerColVisible = companies.some(x => "manager" in x.fields);

  return (
    <>
      {userPermissions.canCreateCompany ? (
        <Link
          className="bg-green-600 p-2 rounded text-white"
          to={routes.companies.new}
        >
          New company
        </Link>
      ) : null}
      <table className="w-full">
        <thead>
          <th className="text-left p-1">Name</th>
          {addressColVisible ? (
            <th className="text-left  p-1">Address</th>
          ) : null}
          {ceoColVisible ? <th className="text-left  p-1">Ceo</th> : null}
          {contactsColVisible ? (
            <th className="text-left  p-1">Contacts</th>
          ) : null}
          {emailColVisible ? <th className="text-left  p-1">Email</th> : null}
          {innColVisible ? <th className="text-left  p-1">Inn</th> : null}
          {phoneColVisible ? <th className="text-left  p-1">Phone</th> : null}
          {typeColVisible ? <th className="text-left  p-1">Type</th> : null}
          {managerColVisible ? (
            <th className="text-left  p-1">Manager</th>
          ) : null}
          <th></th>
        </thead>
        <tbody>
          {companies.length > 0 ? (
            companies.map((x, i) => (
              <tr key={i}>
                <td className="p-1">
                  <Link to={routes.companies.view(x.id)}>
                    {"name" in x.fields
                      ? x.fields.name?.toString()
                      : "<forbidden to see the name>"}
                  </Link>
                </td>
                {addressColVisible ? (
                  "address" in x.fields ? (
                    <td className="p-1">{x.fields.address?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {ceoColVisible ? (
                  "ceo" in x.fields ? (
                    <td className="p-1">{x.fields.ceo?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {contactsColVisible ? (
                  "contacts" in x.fields ? (
                    <td className="p-1">{x.fields.contacts?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {emailColVisible ? (
                  "email" in x.fields ? (
                    <td className="p-1">{x.fields.email?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {innColVisible ? (
                  "inn" in x.fields ? (
                    <td className="p-1">{x.fields.inn?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {phoneColVisible ? (
                  "phone" in x.fields ? (
                    <td className="p-1">{x.fields.phone?.toString()}</td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {typeColVisible ? (
                  "type" in x.fields ? (
                    <td className="p-1">
                      {(x.fields.type as CompanyType)?.name?.toString()}
                    </td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                {managerColVisible ? (
                  "manager" in x.fields ? (
                    <td className="p-1">
                      {x.fields.manager
                        ? `${(x.fields.manager as Manager)?.lastName} ${
                            (x.fields.manager as Manager)?.firstName
                          }`
                        : "-"}
                    </td>
                  ) : (
                    <td></td>
                  )
                ) : null}
                <td className="flex justify-end p-1">
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
                      className="ml-1 text-red-600"
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
