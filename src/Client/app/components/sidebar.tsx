import { Link } from "@remix-run/react";
import { routes } from "~/utils/constants";

export default function Sidebar() {
  return (
    <div className="w-80">
      <nav className="fixed w-80 border-r p-4 h-full">
        <ul>
          <li>
            <Link to={routes.companies.index}>Companies</Link>
          </li>
        </ul>
      </nav>
    </div>
  );
}
