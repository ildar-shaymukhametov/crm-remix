import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json, redirect } from "@remix-run/node";
import {
  isRouteErrorResponse,
  useActionData,
  useLoaderData,
  useRouteError
} from "@remix-run/react";
import { ButtonSuccess } from "~/components/buttons";
import { auth } from "~/utils/auth.server";
import type { NewCompany, NewCompanyVm } from "~/utils/companies.server";
import { createCompany, getNewCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const newCompanyVm = await getNewCompany(request, user.extra?.access_token);

  return json({
    newCompanyVm
  });
};

type ActionData = {
  errors?: {
    [index: string]: string[];
  };
  fields?: { [index: string]: string };
};

type LoaderData = {
  newCompanyVm: NewCompanyVm;
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
  const { newCompanyVm: vm } = useLoaderData<LoaderData>();

  return (
    <form method="post">
      {"name" in vm.fields ? (
        <div>
          <label>
            Name:
            <input name="name" required maxLength={200} />
          </label>
        </div>
      ) : null}
      {"typeId" in vm.fields ? (
        <div>
          <label>
            Type:
            <select name="typeId">
              <option value="">-</option>
              {vm.initData.companyTypes.map(x => (
                <option key={x.id} value={x.id}>
                  {x.name}
                </option>
              ))}
            </select>
          </label>
        </div>
      ) : null}
      {"inn" in vm.fields ? (
        <div>
          <label>
            Inn:
            <input name="inn" />
          </label>
          {data?.errors?.Inn
            ? data.errors.Inn.map((error, i) => <p key={i}>{error}</p>)
            : null}
        </div>
      ) : null}
      {"address" in vm.fields ? (
        <div>
          <label>
            Address:
            <input name="address" />
          </label>
        </div>
      ) : null}
      {"ceo" in vm.fields ? (
        <div>
          <label>
            CEO:
            <input name="ceo" />
          </label>
        </div>
      ) : null}
      {"phone" in vm.fields ? (
        <div>
          <label>
            Phone:
            <input name="phone" />
          </label>
        </div>
      ) : null}
      {"email" in vm.fields ? (
        <div>
          <label>
            Email:
            <input name="email" />
          </label>
          {data?.errors?.Email
            ? data.errors.Email.map((error, i) => <p key={i}>{error}</p>)
            : null}
        </div>
      ) : null}
      {"contacts" in vm.fields ? (
        <div>
          <label>
            Contacts:
            <input name="contacts" />
          </label>
        </div>
      ) : null}
      {"managerId" in vm.fields ? (
        <div>
          <label>
            Manager:
            <select name="managerId">
              {vm.initData.managers.map((x, i) => (
                <option key={i} value={x.id}>
                  {x.firstName && x.lastName
                    ? `${x.lastName} ${x.firstName}`
                    : "-"}
                </option>
              ))}
            </select>
          </label>
        </div>
      ) : null}

      <ButtonSuccess type="submit">Create new company</ButtonSuccess>
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
