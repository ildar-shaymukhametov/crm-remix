import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import {
  isRouteErrorResponse,
  useRouteError,
  useLoaderData,
  useParams
} from "@remix-run/react";
import invariant from "tiny-invariant";
import { LinkDanger, LinkPrimary } from "~/components/buttons";
import { auth } from "~/utils/auth.server";
import type { Company, CompanyType, Manager } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";

type LoaderData = {
  company: Company;
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
      <div className="mb-3">
        {company.canBeUpdated ? (
          <LinkPrimary to={routes.companies.edit(id)}>Edit</LinkPrimary>
        ) : null}
        {company.canBeDeleted ? (
          <LinkDanger to={routes.companies.delete(id)} className="ml-2">
            Delete
          </LinkDanger>
        ) : null}
      </div>
      <div>
        {"name" in company.fields ? (
          <p>
            <span>name</span>:{" "}
            <span aria-label="name">{company.fields.name?.toString()}</span>
          </p>
        ) : null}
        {"type" in company.fields ? (
          <p>
            <span>type</span>:{" "}
            <span aria-label="type">
              {(company.fields.type as CompanyType)?.name}
            </span>
          </p>
        ) : null}
        {"inn" in company.fields ? (
          <p>
            <span>inn</span>:{" "}
            <span aria-label="inn">{company.fields.inn?.toString()}</span>
          </p>
        ) : null}
        {"address" in company.fields ? (
          <p>
            <span>address</span>:{" "}
            <span aria-label="address">
              {company.fields.address?.toString()}
            </span>
          </p>
        ) : null}
        {"ceo" in company.fields ? (
          <p>
            <span>ceo</span>:{" "}
            <span aria-label="ceo">{company.fields.ceo?.toString()}</span>
          </p>
        ) : null}
        {"phone" in company.fields ? (
          <p>
            <span>phone</span>:{" "}
            <span aria-label="phone">{company.fields.phone?.toString()}</span>
          </p>
        ) : null}
        {"email" in company.fields ? (
          <p>
            <span>email</span>:{" "}
            <span aria-label="email">{company.fields.email?.toString()}</span>
          </p>
        ) : null}
        {"contacts" in company.fields ? (
          <p>
            <span>contacts</span>:{" "}
            <span aria-label="contacts">
              {company.fields.contacts?.toString()}
            </span>
          </p>
        ) : null}
        {"manager" in company.fields ? (
          <p>
            <span>manager</span>:{" "}
            <span aria-label="manager">
              {company.fields.manager
                ? `${(company.fields.manager as Manager)?.lastName} ${
                    (company.fields.manager as Manager)?.firstName
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
  if (data?.company?.fields?.name != undefined) {
    return [
      {
        title: data.company.fields.name
      }
    ];
  }

  return [
    {
      title: "View company"
    }
  ];
};
