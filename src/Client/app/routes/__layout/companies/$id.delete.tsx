import type {
  ActionFunction,
  LoaderFunction,
  MetaFunction,
} from "@remix-run/node";
import { json } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { Link, useCatch, useParams } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

type Company = {
  id: number;
  name: string;
};

type LoaderData = {
  company: Company;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, {
    permissions: ["DeleteCompany", "ViewCompany"],
  });
  if (!user.permissions.includes("DeleteCompany")) {
    throw new Response(null, { status: 403 });
  }

  const response = await fetch(
    `${process.env.API_URL}/companies/${params.id}`,
    {
      headers: {
        Authorization: `Bearer ${user.extra?.access_token}`,
      },
    }
  );

  if (!response.ok) {
    throw response;
  }

  const data = await response.json();
  return json({ company: data });

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
          <button type="submit">Delete company</button>
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

export const meta: MetaFunction<LoaderData> = ({ data }) => {
  if (!data?.company) {
    return {
      title: "Delete company",
    };
  }

  return {
    title: `${data.company.name} • Delete`,
  };
};
