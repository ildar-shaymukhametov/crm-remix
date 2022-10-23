import type { LoaderFunction } from "@remix-run/node";
import { json } from "@remix-run/node";
import { Form, useLoaderData } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  let user = await auth.requireUser(request);
  return json(user);
};

export default function Index() {
  const user = useLoaderData();

  return (
    <div>
      <h1>Welcome to Remix {user ? user.id : null}</h1>
      <ul>
        {user ? (
          <li>
            <Form method="post" action="/logout">
              <button>Log Out</button>
            </Form>
          </li>
        ) : null}
        <li>
          <a
            target="_blank"
            href="https://remix.run/tutorials/jokes"
            rel="noreferrer"
          >
            Deep Dive Jokes App Tutorial
          </a>
        </li>
        <li>
          <a target="_blank" href="https://remix.run/docs" rel="noreferrer">
            Remix Docs
          </a>
        </li>
        <li>
          <a href="/private" rel="noreferrer">
            Private
          </a>
        </li>
      </ul>
    </div>
  );
}
