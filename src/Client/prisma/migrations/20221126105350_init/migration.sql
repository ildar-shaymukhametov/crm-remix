-- CreateTable
CREATE TABLE "Session" (
    "id" TEXT NOT NULL PRIMARY KEY,
    "data" TEXT,
    "expires" DATETIME
);

-- CreateIndex
CREATE UNIQUE INDEX "Session_id_key" ON "Session"("id");
