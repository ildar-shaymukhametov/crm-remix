import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useActionData, useCatch, useLoaderData } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";

type Company = {
  id: number;
  type: string;
  name: string;
  inn: string;
  address: string;
  ceo: string;
  phone: string;
  email: string;
  contacts: string;
};

type LoaderData = {
  company: Company;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  if (!user.permissions.includes("company.update")) {
    throw new Response(null, { status: 401, statusText: "Unauthorized" });
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
    if (response.status === 404) {
      throw new Response("Not Found", { status: 404 });
    }

    if (response.status === 401) {
      throw new Response("Unauthorized", { status: 401 });
    }

    return json(null, { status: response.status });
  }

  const data = await response.json();
  return json({ company: data });
};

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: { [index: string]: string | number }
};

export const action: ActionFunction = async ({ request, params }) => {
  invariant(params?.id, "Missing id parameter");

  const formData = await request.formData();
  const data = Object.fromEntries(formData);
  data.id = params.id;

  const user = await auth.requireUser(request);
  const response = await fetch(
    `${process.env.API_URL}/companies/${params.id}`,
    {
      method: "put",
      body: JSON.stringify(data),
      headers: {
        Authorization: `Bearer ${user.extra?.access_token}`,
        "Content-Type": "application/json",
      },
    }
  );

  if (!response.ok) {
    if (response.status === 404) {
      throw new Error("Company not found");
    }

    if (response.status === 400) {
      const responseData = await response.json();
      return json(
        { fields: data, errors: responseData.errors },
        { status: response.status }
      );
    }

    throw new Error(`${response.statusText} (${response.status})`);
  }

  return redirect(`/companies/${params.id}`);
};

export default function EditCompanyRoute() {
  const actionData = useActionData<ActionData>();
  const loaderData = useLoaderData<LoaderData>();
  const data: ActionData = {
    fields: { ...loaderData.company, ...actionData?.fields },
    errors: actionData?.errors,
  };

  return (
    <form method="post">
      <div>
        <label>
          Name:
          <input
            name="name"
            required
            maxLength={200}
            defaultValue={data?.fields?.name}
          />
        </label>
      </div>
      <div>
        <label>
          Type:
          <select name="type" defaultValue={data?.fields?.type}>
            <option value=""></option>
            <option value="1">ООО</option>
            <option value="2">АО</option>
            <option value="3">ПАО</option>
            <option value="4">ИП</option>
          </select>
        </label>
      </div>
      <div>
        <label>
          Inn:
          <input name="inn" defaultValue={data?.fields?.inn} />
        </label>
        {data?.errors?.Inn
          ? data.errors.Inn.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Address:
          <input name="address" defaultValue={data?.fields?.address} />
        </label>
      </div>
      <div>
        <label>
          CEO:
          <input name="ceo" defaultValue={data?.fields?.ceo} />
        </label>
        {data?.errors?.Ceo
          ? data.errors.Ceo.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Phone:
          <input name="phone" defaultValue={data?.fields?.phone} />
        </label>
      </div>
      <div>
        <label>
          Email:
          <input name="email" defaultValue={data?.fields?.email} />
        </label>
        {data?.errors?.Email
          ? data.errors.Email.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Contacts:
          <input name="contacts" defaultValue={data?.fields?.contacts} />
        </label>
      </div>
      <button type="submit">Save</button>
    </form>
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

export function ErrorBoundary({ error }: { error: Error }) {
  console.error(error.message);
  return <p>{error.message}</p>;
}
