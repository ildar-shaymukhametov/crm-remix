import { useLocation } from "@remix-run/react";
import { Form, Link } from "@remix-run/react";
import { getCompaniesRouteNavbarButtons } from "~/routes/companies";
import { getCompanyRouteNavbarButtons } from "~/routes/companies/$id";
import { useUser } from "~/utils/utils";

export default function Navbar() {
  const user = useUser();
  const loc = useLocation();
  const buttons = getNavbarButtons(loc.pathname);

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

function getNavbarButtons(pathname: string) {
  if (pathname === "/companies") {
    return getCompaniesRouteNavbarButtons();
  }

  const match = pathname.match(/^\/companies\/(?<id>[1-9])+$/);
  if (match?.groups?.id) {
    return getCompanyRouteNavbarButtons(match.groups.id);
  }

  return [];
}
