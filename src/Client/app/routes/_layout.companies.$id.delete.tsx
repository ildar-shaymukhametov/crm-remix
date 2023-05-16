import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import { isRouteErrorResponse, useRouteError } from "@remix-run/react";
import { Link, useParams } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import { getCompany, deleteCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

export const loader: LoaderFunction = async ({ request, params }) => {
  invariant(params.id, "Missing id parameter");

  const user = await auth.requireUser(request, {
    key: params.id,
    permissions: [permissions.deleteCompany]
  });
  if (!user.permissions.includes(permissions.deleteCompany)) {
    throw new Response(null, { status: 403 });
  }

  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );
  return json({ company });
};

export const action: ActionFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);

  invariant(params.id, "Missing id parameter");
  await deleteCompany(request, params.id, user.extra?.access_token);

  return redirect(routes.companies.index);
};

export default function DeleteCompany() {
  const params = useParams();

  return (
    <>
      <p>Company will be deleted.</p>
      <div>
        <Link to={routes.companies.view(params.id)} className="mr-2">
          Cancel
        </Link>
        <form method="post">
          <button type="submit">Delete company</button>
        </form>
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
        title: "Delete company"
      }
    ];
  }

  return [
    {
      title: `${data.company.name} • Delete`
    }
  ];
};
