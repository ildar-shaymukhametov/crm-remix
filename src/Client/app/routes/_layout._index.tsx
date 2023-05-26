import type { LoaderFunction} from "@remix-run/node";
import { redirect } from "@remix-run/node";
import { routes } from "~/utils/constants";

export const loader: LoaderFunction = async () => {
  return redirect(routes.companies.index);
};
