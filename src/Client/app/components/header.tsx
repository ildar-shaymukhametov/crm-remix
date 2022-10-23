import { Form, Link, useLocation } from "@remix-run/react";
import { useNavbarButtons } from "~/utils/navbar-buttons";
import { useUser } from "~/utils/utils";

export default function Header() {
  const user = useUser();
  const loc = useLocation();
  const buttons = useNavbarButtons(loc.pathname);

  return (
    <header className="fixed w-full top-0 p-5 bg-slate-200 flex justify-between items-center">
      <div className="w-80">
        {user ? <Link to="/">{user.displayName}</Link> : null}
      </div>
      <div className="flex flex-1 justify-between">
        <div>{buttons}</div>
        <div>
          {user ? (
            <Form method="post" action="/logout">
              <button>Log Out</button>
            </Form>
          ) : null}
        </div>
      </div>
    </header>
  );
}
