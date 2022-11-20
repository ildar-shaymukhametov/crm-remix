import { Link } from "@remix-run/react";

export default function Sidebar() {
  return (
    <div className="w-80">
      <nav className="fixed w-80 border-r p-4 h-full">
        <ul>
          <li>
            <Link to="companies">Companies</Link>
          </li>
        </ul>
      </nav>
    </div>
  );
}
