import type { Cookie } from "@remix-run/node";
import {
  createCookie,
  createSessionStorage,
  createCookieSessionStorage
} from "@remix-run/node";
import invariant from "tiny-invariant";
import crypto from "crypto";
import { prisma } from "~/db.server";

invariant(process.env.SESSION_SECRET, "SESSION_SECRET must be set");

const sessionCookie = createCookie("_session", {
  secure: process.env.NODE_ENV === "production",
  secrets: [process.env.SESSION_SECRET],
  sameSite: "lax",
  path: "/",
  maxAge: 60 * 60 * 24 * 30,
  httpOnly: true
});

export const sessionStorage = createDatabaseSessionStorage({
  cookie: sessionCookie
});

export const returnUrlSession = createCookieSessionStorage({
  cookie: createCookie("_session.returnUrl", {
    secure: process.env.NODE_ENV === "production",
    secrets: [process.env.SESSION_SECRET],
    sameSite: "lax",
    path: "/",
    maxAge: 60,
    httpOnly: true
  })
});

function createDatabaseSessionStorage({ cookie }: { cookie: Cookie }) {
  return createSessionStorage({
    cookie,
    async createData(data, expires?) {
      const session = await prisma.session.create({
        data: {
          id: crypto.randomUUID(),
          data: JSON.stringify(data),
          expires
        }
      });
      console.log(`🟢 Created session with id: ${session.id}.`);
      return session.id;
    },
    async readData(id) {
      if (!id) {
        return;
      }

      try {
        const session = await prisma.session.findUnique({
          where: {
            id
          }
        });

        return session?.data ? JSON.parse(session.data) : null;
      } catch (error) {
        console.log(`🔴 Read session: did not found session with id: ${id}.`);
      }
    },
    async updateData(id, data, expires) {
      if (!id) {
        return;
      }

      try {
        await prisma.session.update({
          where: {
            id
          },
          data: {
            data: JSON.stringify(data),
            expires
          }
        });
        console.log(`🟢 Updated session with id: ${id}.`);
      } catch (error) {
        console.log(`🔴 Update session: did not found session with id: ${id}.`);
      }
    },
    async deleteData(id) {
      if (!id) {
        return;
      }

      try {
        await prisma.session.delete({
          where: {
            id
          }
        });
        console.log(`🟢 Deleted session with id: ${id}.`);
      } catch (error) {
        console.log(`🔴 Delete session: did not found session with id: ${id}.`);
      }
    }
  });
}

export const { getSession, commitSession, destroySession } = sessionStorage;
