import { Link } from "@remix-run/react";

type LinkProps = {
  to: string;
  children: React.ReactNode;
  className?: string;
};

export function LinkDefault({ to, children, className }: LinkProps) {
  return (
    <Link
      className={`bg-gray-600 text-white py-2 px-3 rounded hover:bg-gray-700 ${className}`}
      to={to}
    >
      {children}
    </Link>
  );
}

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

export function LinkPrimary({ to, children }: LinkProps) {
  return (
    <Link
      className="bg-blue-700 text-white py-2 px-3 rounded hover:bg-blue-800"
      to={to}
    >
      {children}
    </Link>
  );
}

export function LinkDanger({ to, children, className }: LinkProps) {
  return (
    <Link
      className={`bg-red-700 text-white py-2 px-3 rounded hover:bg-red-800 ${className}`}
      to={to}
    >
      {children}
    </Link>
  );
}

type ButtonProps = {
  type: "button" | "submit";
  children: React.ReactNode;
  className?: string;
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

export function ButtonDanger({ type, children }: ButtonProps) {
  return (
    <button
      type={type}
      className="bg-red-700 text-white py-2 px-3 rounded hover:bg-red-800"
    >
      {children}
    </button>
  );
}

export function ButtonPrimary({ type, children, className }: ButtonProps) {
  return (
    <button
      type={type}
      className={`bg-blue-700 text-white py-2 px-3 rounded hover:bg-blue-800 ${className}`}
    >
      {children}
    </button>
  );
}
