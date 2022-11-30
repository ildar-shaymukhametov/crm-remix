import type {
  ActionFunction,
  LoaderFunction,
  MetaFunction
} from "@remix-run/node";
import { json } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { Link, useCatch, useParams } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company } from "~/utils/companies.server";
import { getCompany, deleteCompany } from "~/utils/companies.server";
import { permissions } from "~/utils/constants";

type LoaderData = {
  company: Company;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.deleteCompany, permissions.viewCompany]
  });
  if (!user.permissions.includes(permissions.deleteCompany)) {
    throw new Response(null, { status: 403 });
  }

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

  return redirect("/companies");
};

export default function DeleteCompany() {
  const params = useParams();

  return (
    <>
      <p>Company will be deleted.</p>
      <div>
        <Link to={`/companies/${params.id}`} className="mr-2">
          Cancel
        </Link>
        <form method="post">
          <button type="submit">Delete company</button>
        </form>
      </div>
    </>
  );
}

export function CatchBoundary() {
  const res = useCatch();
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
      title: "Delete company"
    };
  }

  return {
    title: `${data.company.name} • Delete`
  };
};
