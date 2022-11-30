import invariant from "tiny-invariant";

declare global {
  // eslint-disable-next-line @typescript-eslint/no-namespace
  namespace NodeJS {
    interface ProcessEnv {
      CLIENT_ID: string;
      CLIENT_SECRET: string;
      AUTHORITY: string;
      ENDSESSION_URL: string;
      USERINFO_URL: string;
      CALLBACK_URL: string;
      POST_LOGOUT_REDIRECT_URI: string;
      API_URL: string;
      DATABASE_URL: string;
    }
  }
}

export function init() {
  const requiredServerEnvs = [
    "CLIENT_ID",
    "CLIENT_SECRET",
    "AUTHORITY",
    "ENDSESSION_URL",
    "USERINFO_URL",
    "CALLBACK_URL",
    "POST_LOGOUT_REDIRECT_URI",
    "API_URL",
    "DATABASE_URL"
  ] as const;
  for (const env of requiredServerEnvs) {
    invariant(process.env[env], `${env} is required`);
  }
}

/**
 * This is used in both `entry.server.ts` and `root.tsx` to ensure that
 * the environment variables are set and globally available before the app is
 * started.
 *
 * NOTE: Do *not* add any environment variables in here that you do not wish to
 * be included in the client.
 * @returns all public ENV variables
 */
export function getEnv() {
  invariant(process.env.NODE_ENV, "NODE_ENV should be defined");

  return {
    MODE: process.env.NODE_ENV
  };
}

type ENV = ReturnType<typeof getEnv>;

declare global {
  var ENV: ENV;
  interface Window {
    ENV: ENV;
  }
}
