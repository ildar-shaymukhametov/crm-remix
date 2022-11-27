import { useMatches } from "@remix-run/react";
import { useMemo } from "react";
import { auth } from "./auth.server";
import type { OidcProfile } from "./oidc-strategy";

/**
 * This base hook is used in other hooks to quickly search for specific data
 * across all loader data using useMatches.
 * @param {string} id The route id
 * @returns {JSON|undefined} The router data or undefined if not found
 */
export function useMatchesData(
  id: string
): Record<string, unknown> | undefined {
  const matchingRoutes = useMatches();
  const route = useMemo(
    () => matchingRoutes.find(route => route.id === id),
    [matchingRoutes, id]
  );
  return route?.data;
}

function isUser(user: OidcProfile) {
  return (
    user && typeof user === "object" && typeof user.displayName === "string"
  );
}

export function useOptionalUser(): OidcProfile | undefined {
  const data = useMatchesData("root");
  if (!data || !isUser(data.user as OidcProfile)) {
    return undefined;
  }

  return data.user as OidcProfile;
}

export function useUser(): OidcProfile {
  const maybeUser = useOptionalUser();
  if (!maybeUser) {
    throw new Error(
      "No user found in root loader, but user is required by useUser. If user is optional, try useOptionalUser instead."
    );
  }
  return maybeUser;
}

export async function handleErrorResponse(
  request: Request,
  response: Response
) {
  if (response.status === 401) {
    return await handle401Response(request);
  }

  throw response;
}

export async function handle401Response(request: Request): Promise<never> {
  return await auth.logout(request, {
    redirectTo: "/login",
    rememberReturnUrl: true
  });
}
