import type {
  ActionFunction,
  LoaderFunction,
  MetaFunction
} from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useActionData, useCatch, useLoaderData } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company } from "~/utils/companies.server";
import { updateCompany } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";

type LoaderData = {
  company: Company;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request, {
    permissions: ["UpdateCompany", "ViewCompany"]
  });
  if (!user.permissions.includes("UpdateCompany")) {
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

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: { [index: string]: string | number };
};

export const action: ActionFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  invariant(params.id, "Missing id parameter");

  const formData = await request.formData();
  const data = Object.fromEntries(formData);
  await updateCompany(request, params.id, data, user.extra?.access_token);

  return redirect(`/companies/${params.id}`);
};

export default function EditCompanyRoute() {
  const actionData = useActionData<ActionData>();
  const loaderData = useLoaderData<LoaderData>();
  const data: ActionData = {
    fields: { ...loaderData.company, ...actionData?.fields },
    errors: actionData?.errors
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
            <option value="ООО">ООО</option>
            <option value="АО">АО</option>
            <option value="ПАО">ПАО</option>
            <option value="ИП">ИП</option>
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
      <button type="submit">Save changes</button>
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

export function ErrorBoundary({ error }: { error: Error }) {
  console.error(error.message);
  return <p>{error.message}</p>;
}

export const meta: MetaFunction<LoaderData> = ({ data }) => {
  if (!data?.company) {
    return {
      title: "Edit company"
    };
  }

  return {
    title: data.company.name
  };
};
