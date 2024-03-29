import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { redirect, json } from "@remix-run/node";
import type { V2_MetaFunction } from "@remix-run/react";
import {
  isRouteErrorResponse,
  useRouteError,
  useLoaderData
} from "@remix-run/react";
import invariant from "tiny-invariant";
import { ButtonPrimary } from "~/components/buttons";
import { auth } from "~/utils/auth.server";
import type { UpdateCompanyQuery } from "~/utils/companies.server";
import { getUpdateCompanyData, updateCompany } from "~/utils/companies.server";
import { routes } from "~/utils/constants";

type LoaderData = {
  company: UpdateCompanyQuery;
};

export const loader: LoaderFunction = async ({ request, params }) => {
  invariant(params.id, "Missing id parameter");

  const user = await auth.requireUser(request);
  const company = await getUpdateCompanyData(
    request,
    params.id,
    user.extra?.access_token
  );

  return json({
    company
  });
};

export const action: ActionFunction = async ({ request, params }) => {
  const user = await auth.requireUser(request);
  invariant(params.id, "Missing id parameter");

  const data = Object.fromEntries(await request.formData());
  if (!data.managerId) {
    delete data.managerId;
  }

  await updateCompany(request, params.id, data, user.extra?.access_token);

  return redirect(routes.companies.view(params.id));
};

export default function EditCompanyRoute() {
  const { company: data } = useLoaderData<LoaderData>();

  return (
    <form method="post">
      {"name" in data.fields ? (
        <div className="mb-3">
          <label>
            Name:
            <input
              name="name"
              required
              maxLength={200}
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.name?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"typeId" in data.fields ? (
        <div className="mb-3">
          <label>
            Type:
            <select
              name="typeId"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.typeId?.toString()}
            >
              <option value="">-</option>
              {data.initData.companyTypes.map(x => (
                <option key={x.id} value={x.id}>
                  {x.name}
                </option>
              ))}
            </select>
          </label>
        </div>
      ) : null}
      {"inn" in data.fields ? (
        <div className="mb-3">
          <label>
            Inn:
            <input
              name="inn"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.inn?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"address" in data.fields ? (
        <div className="mb-3">
          <label>
            Address:
            <input
              name="address"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.address?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"ceo" in data.fields ? (
        <div className="mb-3">
          <label>
            CEO:
            <input
              name="ceo"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.ceo?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"phone" in data.fields ? (
        <div className="mb-3">
          <label>
            Phone:
            <input
              name="phone"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.phone?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"email" in data.fields ? (
        <div className="mb-3">
          <label>
            Email:
            <input
              name="email"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.email?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"contacts" in data.fields ? (
        <div className="mb-3">
          <label>
            Contacts:
            <input
              name="contacts"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.contacts?.toString()}
            />
          </label>
        </div>
      ) : null}
      {"managerId" in data.fields ? (
        <div className="mb-3">
          <label>
            Manager:
            <select
              name="managerId"
              className="rounded border border-gray-300 w-full p-1"
              defaultValue={data.fields.managerId?.toString()}
            >
              {data.initData.managers.map((x, i) => (
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

      <ButtonPrimary type="submit" className="mt-3">
        Save changes
      </ButtonPrimary>
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
  if (data?.company?.fields?.name != undefined) {
    return [
      {
        title: data.company.fields.name
      }
    ];
  }

  return [
    {
      title: "Edit company"
    }
  ];
};
