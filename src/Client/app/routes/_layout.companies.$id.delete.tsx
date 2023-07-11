import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect, json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import {
  isRouteErrorResponse,
  useRouteError,
  useParams
} from "@remix-run/react";
import invariant from "tiny-invariant";
import { ButtonDanger, LinkDefault } from "~/components/buttons";
import { auth } from "~/utils/auth.server";
import { getCompany, deleteCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  invariant(params.id, "Missing id parameter");

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
      <p className="mb-3">Company will be deleted.</p>
      <div className="flex">
        <LinkDefault to={routes.companies.view(params.id)} className="mr-2">
          Cancel
        </LinkDefault>
        <form method="post">
          <ButtonDanger type="submit">Delete company</ButtonDanger>
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
  if (data?.company?.fields?.name != undefined) {
    return [
      {
        title: data.company.fields.name
      }
    ];
  }

  return [
    {
      title: "Delete company"
    }
  ];
};
