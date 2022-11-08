import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { Link, useCatch, useParams } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  let user = await auth.requireUser(request);
  if (!user.permissions.includes("company.delete")) {
    throw new Response(null, { status: 403, statusText: "Forbidden" });
  }

  return user;
};

export const action: ActionFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(
    `${process.env.API_URL}/companies/${params.id}`,
    {
      method: "delete",
      headers: {
        Authorization: `Bearer ${user.extra.access_token}`,
      },
    }
  );

  if (!response.ok) {
    throw response;
  }

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
  if (res.status === 403) {
    return <p>Forbidden</p>;
  }
  if (res.status === 404) {
    return <p>Company not found</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${res.status}`);
}
