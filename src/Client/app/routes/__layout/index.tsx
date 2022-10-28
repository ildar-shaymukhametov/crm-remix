import type { LoaderFunction } from "@remix-run/node";
import { Outlet } from "@remix-run/react";
import { auth } from "~/utils/auth.server";

export const loader: LoaderFunction = async ({ request }) => {
  return await auth.requireUser(request);
};

export default function Index() {
  return "Hello";
}