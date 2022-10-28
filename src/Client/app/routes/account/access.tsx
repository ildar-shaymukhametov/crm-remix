import type { ActionFunction, LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { useCatch, useLoaderData, useSubmit } from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import Select from "react-select";
import { useRef } from "react";

type ClaimType = {
  id: number;
  name: string;
  value: string;
};

type LoaderData = {
  claimTypes: ClaimType[];
  claims: string[]
};

export const action: ActionFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const formData = await request.formData();
  const data = {
    claims: Object.values(Object.fromEntries(formData)),
  };
  const response = await fetch(`${process.env.API_URL}/Users/Claims`, {
    method: "post",
    body: JSON.stringify(data),
    headers: {
      Authorization: `Bearer ${user.extra?.accessToken}`,
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    throw new Error(`${response.statusText} (${response.status})`);
  }

  return null;
};

export const loader: LoaderFunction = async ({ request }) => {
  const user = await auth.requireUser(request);
  const response = await fetch(`${process.env.API_URL}/UserClaimTypes`, {
    headers: {
      Authorization: `Bearer ${user.extra?.accessToken}`,
    },
  });
  const userClaimsResponse = await fetch(`${process.env.API_URL}/Users/Claims`, {
    headers: {
      Authorization: `Bearer ${user.extra?.accessToken}`,
    },
  });

  if (!response.ok) {
    if (response.status === 404) {
      throw new Response("Not Found", { status: 404 });
    }

    if (response.status === 401) {
      throw new Response("Unauthorized", { status: 401 });
    }

    return json(null, { status: response.status });
  }

  const claimTypes = await response.json();
  const claims = await userClaimsResponse.json();
  return json({ claimTypes, claims });
};

export default function AccessRoute() {
  const submit = useSubmit();
  const selectElem = useRef(null);
  const { claimTypes, claims } = useLoaderData<LoaderData>();
  const options = claimTypes.map((x) => ({
    value: x.value,
    label: x.name,
  }));
  const selectedOptions = options.filter(x => claims.includes(x.value));

  const onButtonClick = () => {
    const formData = new FormData();
    const items = selectElem.current.getValue() as [{ value: string }];
    items.forEach((item, index) => {
      formData.append(index.toString(), item.value);
    });

    submit(formData, { method: "post" });
  };

  return (
    <>
      <Select
        ref={selectElem}
        instanceId="user-claims"
        isMulti
        options={options}
        defaultValue={selectedOptions}
      />
      <button type="button" onClick={onButtonClick}>
        Save
      </button>
    </>
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
  return <p>Unexpected error</p>;
}
