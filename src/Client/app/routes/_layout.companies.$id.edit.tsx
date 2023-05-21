import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import { isRouteErrorResponse, useRouteError } from "@remix-run/react";
import { useActionData, useLoaderData } from "@remix-run/react";
import invariant from "tiny-invariant";
import { auth } from "~/utils/auth.server";
import type { Company, Manager } from "~/utils/companies.server";
import { getInitData } from "~/utils/companies.server";
import { updateCompany } from "~/utils/companies.server";
import { getCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

type LoaderData = {
  company: Company;
  managers?: Manager[];
};

export const loader: LoaderFunction = async ({ request, params }) => {
  invariant(params.id, "Missing id parameter");

  const user = await auth.requireUser(request, {
    key: params.id,
    permissions: [permissions.updateCompany]
  });
  if (!user.permissions.includes(permissions.updateCompany)) {
    throw new Response(null, { status: 403 });
  }

  const company = await getCompany(
    request,
    params.id,
    user.extra?.access_token
  );

  const initData = await getInitData(request, user.extra?.access_token);

  return json({ company, ...initData });
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

  return redirect(routes.companies.view(params.id));
};

export default function EditCompanyRoute() {
  const actionData = useActionData<ActionData>();
  const { company, managers } = useLoaderData<LoaderData>();
  const data: ActionData = {
    fields: {
      ...{
        address: company.address,
        ceo: company.ceo,
        contacts: company.contacts,
        email: company.email,
        id: company.id,
        inn: company.inn,
        phone: company.phone,
        type: company.type,
        name: company.name
      },
      ...actionData?.fields
    },
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
      <div>
        <label>
          Manager:
          <select name="managerId">
            {managers
              ? managers.map((x, i) => (
                  <option key={i} value={x.id}>
                    {x.firstName && x.lastName
                      ? `${x.firstName} ${x.lastName}`
                      : "-"}
                  </option>
                ))
              : null}
          </select>
        </label>
      </div>
      <button type="submit">Save changes</button>
    </form>
  );
}

export function ErrorBoundary() {
  const error = useRouteError();
  if (!isRouteErrorResponse(error)) {
    return <p>Unexpected error</p>;
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
        title: "Edit company"
      }
    ];
  }

  return [
    {
      title: data.company.name
    }
  ];
};
