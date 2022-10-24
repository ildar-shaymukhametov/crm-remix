import type { RouteMatch } from "@remix-run/react";
import { Form, Link, useMatches } from "@remix-run/react";
import { useUser } from "~/utils/utils";

export default function Header() {
  const user = useUser();
  const matches = useMatches();
  const buttons = getNavbarButtons(matches);

  return (
    <header className="fixed w-full top-0 p-5 bg-slate-200 flex justify-between items-center">
      <div className="w-80">
        {user ? <Link to="/">{user.displayName}</Link> : null}
      </div>
      <div className="flex flex-1">
        {buttons.length > 0 ? (
          <ul className="flex">
            {buttons.map((button, index) => (
              <li className="mr-2" key={index}>
                {button}
              </li>
            ))}
          </ul>
        ) : null}
        <div className="ml-auto">
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

function getNavbarButtons(matches: RouteMatch[]) {
  return matches
    .filter((x) => x.handle && x.handle.navbarButtons)
    .flatMap(({ handle }) => handle!.navbarButtons);
}
