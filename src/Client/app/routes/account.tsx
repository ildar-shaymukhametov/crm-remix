import { Outlet } from "@remix-run/react";
import AccountSidebar from "~/components/account-sidebar";

export default function AccountLayout() {
  return (
    <>
      <AccountSidebar />
      <div className="flex-1 p-4">
        <Outlet />
      </div>
    </>
  );
}
