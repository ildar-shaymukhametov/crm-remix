import { Form, Link } from "@remix-run/react";
import { useUser } from "~/utils/utils";

export default function Navbar() {
  const user = useUser();

  return (
    <header className="fixed w-full top-0 p-5 bg-slate-200 flex justify-between items-center">
      <div className="w-80">
        <Link to="/">Crm</Link>
      </div>
      <div className="flex flex-1">
        <div className="ml-auto flex">
          {user ? (
            <Link to="account/profile" className="mr-3">
              {user.displayName}
            </Link>
          ) : null}
          {user ? (
            <Form method="POST" action="/logout">
              <button>Log Out</button>
            </Form>
          ) : null}
        </div>
      </div>
    </header>
  );
}
