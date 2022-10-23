import { Form, Link } from "@remix-run/react";
import { useUser } from "~/utils/utils";

export default function Header() {
  const user = useUser();

  return (
    <header className="fixed w-full top-0 p-5 bg-slate-200 flex justify-between items-center">
      {user ? <Link to="/">{user.displayName}</Link> : null}
      {user ? (
        <Form method="post" action="/logout">
          <button>Log Out</button>
        </Form>
      ) : null}
    </header>
  );
}
