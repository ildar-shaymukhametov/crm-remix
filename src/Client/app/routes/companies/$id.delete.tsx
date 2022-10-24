import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json, redirect } from "@remix-run/node";
import { Link, useActionData, useParams } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.requireUser(request);
};

export const action: ActionFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(`${process.env.API_URL}/companies/${params.id}`, {
    method: "delete",
    headers: {
      Authorization: `Bearer ${user.extra?.accessToken}`,
    },
  });

  if (!response.ok) {
    return json({ ok: false }, { status: response.status });
  }

  return redirect("/companies");
};

export default function DeleteCompany() {
  const params = useParams();
  const data = useActionData();
  if (data && !data.ok) {
    return <p>Failed to delete company</p>;
  }

  return (
    <>
      <p>Company will be deleted.</p>
      <div>
        <Link to={`/companies/${params.id}`} className="mr-2">
          Cancel
        </Link>
        <form method="post">
          <button type="submit">Delete</button>
        </form>
      </div>
    </>
  );
}
