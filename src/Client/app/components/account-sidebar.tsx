import { Link } from "@remix-run/react";
import { routes } from "~/utils/constants";

export default function AccountSidebar() {
  return (
    <div className="w-80">
      <nav className="fixed w-80 border-r p-4 h-full">
        <ul>
          <li>
            <Link to={routes.account.profile}>Profile</Link>
          </li>
          <li>
            <Link to={routes.account.access}>Access rights</Link>
          </li>
        </ul>
      </nav>
    </div>
  );
}
