import { useUser } from "~/utils/utils";

export default function Screen() {
  const user = useUser();
  return (
    <pre className="whitespace-pre-wrap break-all">{JSON.stringify(user, null, 2)}</pre>
  );
}
