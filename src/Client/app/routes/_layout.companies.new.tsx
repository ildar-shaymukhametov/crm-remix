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
import type {
  CompanyType,
  Manager,
  NewCompany
} from "~/utils/companies.server";
import { createCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";
import { permissions } from "~/utils/constants.server";

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request, {
    permissions: [permissions.company.create]
  });

  if (!user.permissions.includes(permissions.company.create)) {
    throw new Response(null, { status: 403 });
  }

  const initData = { managers: [""], companyTypes: [""] };

  return json({
    ...initData
  });
};

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: { [index: string]: string };
};

type LoaderData = {
  managers?: Manager[];
  userPermissions: {
    canSetManager: boolean;
  };
  companyTypes: CompanyType[];
};

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const formData = await request.formData();
  const data = Object.fromEntries(formData) as unknown as NewCompany;

  if (!data.managerId) {
    delete data.managerId;
  }

  if (!data.typeId) {
    delete data.typeId;
  }

  const id = await createCompany(request, data, user.extra?.access_token);

  return redirect(routes.companies.view(id));
};

export default function NewCompanyRoute() {
  const data = useActionData<ActionData>();
  const { managers, companyTypes } = useLoaderData<LoaderData>();

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
          <select name="typeId">
            <option value="">-</option>
            {companyTypes.map(x => (
              <option key={x.id} value={x.id}>
                {x.name}
              </option>
            ))}
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
      {managers && managers.length > 0 ? (
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
      ) : null}

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
