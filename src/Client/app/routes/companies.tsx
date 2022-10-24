import type { LoaderFunction } from "@remix-run/node";
import { Link, Outlet } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.isAuthenticated(request);
};

export const handle = {
  navbarButtons: [<Link to="companies/new">New company</Link>],
};

export default function CompaniesRoute() {
  return <Outlet />
}
