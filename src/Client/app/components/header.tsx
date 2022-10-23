import { Form } from "@remix-run/react";
import { useUser } from "~/utils/utils";

export default function Header() {
  const user = useUser();

  return (
    <header className="p-5 bg-slate-200 flex justify-between items-center">
      {user ? user.displayName : null}
      {user ? (
        <Form method="post" action="/logout">
          <button>Log Out</button>
        </Form>
      ) : null}
    </header>
  );
}
