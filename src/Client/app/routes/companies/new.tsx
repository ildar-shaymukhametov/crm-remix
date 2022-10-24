import type { LoaderFunction } from "@remix-run/node";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = ({ request }) => {
  return auth.isAuthenticated(request);
};

export default function NewCompanyRoute() {
  return "New";
}