import { PrivateNavbarButtons } from "~/routes/private";

export function useNavbarButtons(path: string) {
  switch (path) {
    case "/private":
      return <PrivateNavbarButtons />;
    default:
      break;
  }
}
