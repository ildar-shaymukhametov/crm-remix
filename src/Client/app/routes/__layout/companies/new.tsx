import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { useActionData, useCatch } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: ["CreateCompany"],
  });
  if (!user.permissions.includes("CreateCompany")) {
    throw new Response(null, { status: 403 });
  }

  return user;
};

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: any;
};

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const formData = await request.formData();
  const data = Object.fromEntries(formData);
  const response = await fetch(`${process.env.API_URL}/companies`, {
    method: "post",
    body: JSON.stringify(data),
    headers: {
      Authorization: `Bearer ${user.extra?.access_token}`,
      "Content-Type": "application/json",
    },
  });

  const responseData = await response.json();
  if (!response.ok) {
    throw response;
  }

  return redirect(responseData.id);
};

export default function NewCompanyRoute() {
  const data = useActionData<ActionData>();

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
      <button type="submit">Create</button>
    </form>
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