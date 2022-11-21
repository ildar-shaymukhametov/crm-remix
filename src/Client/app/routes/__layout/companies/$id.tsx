import type { LoaderFunction, MetaFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Link, useCatch, useLoaderData } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company} from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import type { OidcProfile } from "~/utils/oidc-strategy";

type LoaderData = {
  company: Company;
  user: OidcProfile;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, {
    permissions: ["ViewCompany", "UpdateCompany", "DeleteCompany"]
  });
  if (!user.permissions.includes("ViewCompany")) {
    throw new Response(null, { status: 403 });
  }

  invariant(params.id, "Missing id parameter");
  const company = await getCompany(params.id, user.extra?.access_token);
  return json({ company, user });
};

export default function CompanyRoute() {
  const { company, user } = useLoaderData<LoaderData>();

  return (
    <>
      {user.permissions.includes("UpdateCompany") ? (
        <Link to="edit">Edit</Link>
      ) : null}
      {user.permissions.includes("DeleteCompany") ? (
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
  if (res.status === 401) {
    return <p>Unauthorized</p>;
  }
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
