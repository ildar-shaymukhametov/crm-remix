import { Outlet } from "@remix-run/react";
import Sidebar from "~/components/sidebar";

export default function Layout() {
  return (
    <>
      <Sidebar />
      <div className="flex-1 p-4">
        <Outlet />
      </div>
    </>
  );
}