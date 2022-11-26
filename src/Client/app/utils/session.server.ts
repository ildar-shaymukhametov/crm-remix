import { createCookie, createSessionStorage } from "@remix-run/node";
import invariant from "tiny-invariant";
import crypto from "crypto";
import { prisma } from "~/db.server";

invariant(process.env.SESSION_SECRET, "SESSION_SECRET must be set");

const cookie = createCookie("_session", {
  secure: process.env.NODE_ENV === "production",
  secrets: [process.env.SESSION_SECRET],
  sameSite: "lax",
  path: "/",
  maxAge: 60 * 60 * 24 * 30,
  httpOnly: true
});

export const sessionStorage = createSessionStorage({
  cookie,
  async createData(data, expires?) {
    const session = await prisma.session.create({
      data: {
        id: crypto.randomUUID(),
        data: JSON.stringify(data),
        expires
      }
    });
    return session.id;
  },
  async readData(id) {
    const session = await prisma.session.findUnique({
      where: {
        id
      }
    });

    return session?.data ? JSON.parse(session.data) : null;
  },
  async updateData(id, data, expires) {
    await prisma.session.update({
      where: {
        id
      },
      data: {
        data: JSON.stringify(data),
        expires
      }
    });
  },
  async deleteData(id) {
    await prisma.session.delete({
      where: {
        id
      }
    });
  }
});

export const { getSession, commitSession, destroySession } = sessionStorage;
