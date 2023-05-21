import { test as base } from "./test";
import type { ClaimType} from "~/utils/account.server";
import { getTestClaimTypes } from "~/utils/account.server";

export const test = base.extend<{
  getClaimTypes: () => Promise<ClaimType[]>;
}>({
  getClaimTypes: [
    async ({ page }, use) => {
      use(async () => {
        return await getTestClaimTypes(
          new Request("http://foobar.com")
        );
      });
    },
    { auto: true }
  ]
});
