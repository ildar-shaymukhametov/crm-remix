import { Link } from "@remix-run/react";

type LinkProps = {
  to: string;
  children: any;
};

export function LinkSuccess({ to, children }: LinkProps) {
  return (
    <Link
      className="bg-green-700 text-white py-2 px-3 rounded hover:bg-green-800"
      to={to}
    >
      {children}
    </Link>
  );
}

type ButtonProps = {
  type: "button" | "submit";
  children: any;
};

export function ButtonSuccess({ type, children }: ButtonProps) {
  return (
    <button
      type={type}
      className="bg-green-700 text-white py-2 px-3 rounded hover:bg-green-800"
    >
      {children}
    </button>
  );
}
