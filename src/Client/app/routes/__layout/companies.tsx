import type { LoaderFunction } from "@remix-run/node";
import { Link, Outlet } from "@remix-run/react";
import { auth } from "~/utils/auth.server";
import type { OidcProfile } from "~/utils/oidc-strategy";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.requireUser(request);
};

export function getCompaniesRouteNavbarButtons(user: OidcProfile) {
  let result = [];
  result.push(<Link to="companies/new">New company</Link>);
  return result;
}

export default function CompaniesRoute() {
  return <Outlet />;
}
