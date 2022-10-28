import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json, redirect } from "@remix-run/node";
import { Link, useActionData, useCatch, useParams } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  let user = await auth.requireUser(request);
  if (!user.authrizationClaims.includes("company.delete")) {
    throw new Response(null, { status: 401, statusText: "Unauthorized" });
  }

  return user;
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

export function CatchBoundary() {
  const res = useCatch();
  if (res.status === 401) {
    return <p>Unauthorized</p>;
  }
  if (res.status === 404) {
    return <p>Company not found</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${res.status}`);
}
