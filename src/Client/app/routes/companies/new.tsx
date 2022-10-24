import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useActionData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.isAuthenticated(request);
};

export const action: ActionFunction = async ({ request }) => {
  const formData = await request.formData();
  const data = Object.fromEntries(formData);

  return json(data);
};

export default function NewCompanyRoute() {
  const data = useActionData();

  return (
    <form method="post">
      <div>
        <label>
          Name:
          <input
            name="name"
            required
            maxLength={200}
            defaultValue={data?.name}
          />
        </label>
      </div>
      <div>
        <label>
          Type:
          <select name="type" defaultValue={data?.type}>
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
          <input name="inn" defaultValue={data?.inn} />
        </label>
      </div>
      <div>
        <label>
          Address:
          <input name="address" defaultValue={data?.address} />
        </label>
      </div>
      <div>
        <label>
          CEO:
          <input name="ceo" defaultValue={data?.ceo} />
        </label>
      </div>
      <div>
        <label>
          Phone:
          <input name="phone" defaultValue={data?.phone} />
        </label>
      </div>
      <div>
        <label>
          Email:
          <input name="email" defaultValue={data?.email} />
        </label>
      </div>
      <div>
        <label>
          Contacts:
          <input name="contacts" defaultValue={data?.contacts} />
        </label>
      </div>
      <button type="submit">Create</button>
    </form>
  );
}
