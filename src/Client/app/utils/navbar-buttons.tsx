export function useNavbarButtons(path: string) {
  switch (path) {
    case "/private":
      return <button>Private</button>;
    default:
      break;
  }
}
