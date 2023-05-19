import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { redirect } from "@remix-run/node";
import {
  isRouteErrorResponse,
  useActionData,
  useLoaderData,
  useRouteError
} from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { Manager, NewCompany } from "~/utils/companies.server";
import { createCompany, getInitData } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.createCompany]
  });

  if (!user.permissions.includes(permissions.createCompany)) {
    throw new Response(null, { status: 403 });
  }

  const initData = await getInitData(request, user.extra?.access_token);

  return json(initData);
};

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: { [index: string]: string };
};

type LoaderData = {
  managers?: Manager[];
};

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const formData = await request.formData();
  const data = Object.fromEntries(formData) as unknown as NewCompany;
  const id = await createCompany(request, data, user.extra?.access_token);

  return redirect(routes.companies.view(id));
};

export default function NewCompanyRoute() {
  const data = useActionData<ActionData>();
  const { managers } = useLoaderData<LoaderData>();

  return (
    <form method="post">
      <div>
        <label>
          Name:
          <input name="name" required maxLength={200} />
        </label>
      </div>
      <div>
        <label>
          Type:
          <select name="type">
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
          <input name="inn" />
        </label>
        {data?.errors?.Inn
          ? data.errors.Inn.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Address:
          <input name="address" />
        </label>
      </div>
      <div>
        <label>
          CEO:
          <input name="ceo" />
        </label>
        {data?.errors?.Ceo
          ? data.errors.Ceo.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Phone:
          <input name="phone" />
        </label>
      </div>
      <div>
        <label>
          Email:
          <input name="email" />
        </label>
        {data?.errors?.Email
          ? data.errors.Email.map((error, i) => <p key={i}>{error}</p>)
          : null}
      </div>
      <div>
        <label>
          Contacts:
          <input name="contacts" />
        </label>
      </div>
      <div>
        <label>
          Manager:
          <select name="manager">
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
      <button type="submit">Create new company</button>
    </form>
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
    return <p>Not found</p>;
  }

  throw new Error(`Unsupported thrown response status code: ${error.status}`);
}

export function meta() {
  return [
    {
      title: "New company"
    }
  ];
}
