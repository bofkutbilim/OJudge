using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OJudge.Migrations;

public partial class JwtSessionsAndRoles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            LOCK TABLE "Roles", "Users" IN SHARE ROW EXCLUSIVE MODE;
            DO $migration$
            DECLARE
                user_role integer;
                admin_role integer;
                role_sequence regclass := pg_get_serial_sequence('"Roles"', 'Id')::regclass;
                duplicate_name text;
            BEGIN
                SELECT "NickName" INTO duplicate_name FROM "Users"
                GROUP BY "NickName" HAVING count(*) > 1 ORDER BY "NickName" LIMIT 1;
                IF FOUND THEN
                    RAISE EXCEPTION 'Duplicate nickname "%". Resolve duplicate nicknames before applying JwtSessionsAndRoles.', duplicate_name;
                END IF;

                SELECT "Id" INTO user_role FROM "Roles" WHERE lower(btrim("Title")) = 'user'
                ORDER BY ("Title" = 'user') DESC, "Id" LIMIT 1;
                SELECT "Id" INTO admin_role FROM "Roles" WHERE lower(btrim("Title")) = 'admin'
                ORDER BY ("Title" = 'admin') DESC, "Id" LIMIT 1;

                IF user_role IS NULL OR admin_role IS NULL THEN
                    -- Explicitly supplied IDs in a legacy database may be ahead of its identity sequence.
                    PERFORM setval(role_sequence, greatest(
                        (SELECT coalesce(max("Id")::bigint + 1, 1) FROM "Roles"), nextval(role_sequence)), false);
                END IF;
                IF user_role IS NULL THEN
                    INSERT INTO "Roles" ("Title") VALUES ('user') RETURNING "Id" INTO user_role;
                END IF;
                IF admin_role IS NULL THEN
                    INSERT INTO "Roles" ("Title") VALUES ('admin') RETURNING "Id" INTO admin_role;
                END IF;

                WITH assignments AS (
                    SELECT u."Id", CASE WHEN lower(btrim(r."Title")) = 'admin' THEN admin_role ELSE user_role END AS target
                    FROM "Users" u LEFT JOIN "Roles" r ON r."Id" = u."RoleId"
                )
                UPDATE "Users" u SET "RoleId" = a.target FROM assignments a
                WHERE u."Id" = a."Id" AND u."RoleId" IS DISTINCT FROM a.target;

                -- Only remove identical, now unreferenced system-role duplicates.
                DELETE FROM "Roles" WHERE ("Title" = 'user' AND "Id" <> user_role)
                                       OR ("Title" = 'admin' AND "Id" <> admin_role);
                UPDATE "Roles" SET "Title" = 'user' WHERE "Id" = user_role;
                UPDATE "Roles" SET "Title" = 'admin' WHERE "Id" = admin_role;
            END;
            $migration$;
            -- Do not reactivate an IP-based login, even if the old version is deployed again.
            UPDATE "Tokens" SET "IsActive" = FALSE WHERE "IsActive" IS TRUE;
            """);

        migrationBuilder.DropForeignKey("FK_Tokens_Users_UserId", "Tokens");
        migrationBuilder.AddForeignKey("FK_Tokens_Users_UserId", "Tokens", "UserId",
            "Users", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
        migrationBuilder.DropForeignKey("FK_Users_Roles_RoleId", "Users");
        migrationBuilder.AlterColumn<int>("RoleId", "Users", type: "integer", nullable: false,
            oldClrType: typeof(int), oldType: "integer", oldNullable: true);
        migrationBuilder.AddForeignKey("FK_Users_Roles_RoleId", "Users", "RoleId",
            "Roles", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        migrationBuilder.CreateIndex("IX_Users_NickName", "Users", "NickName", unique: true);
        migrationBuilder.CreateIndex("IX_Roles_Title", "Roles", "Title", unique: true,
            filter: "\"Title\" IN ('admin', 'user')");

        migrationBuilder.CreateTable(
            name: "JwtSessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<int>(type: "integer", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_JwtSessions", row => row.Id);
                table.ForeignKey("FK_JwtSessions_Users_UserId", row => row.UserId,
                    "Users", "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex("IX_JwtSessions_ExpiresAt", "JwtSessions", "ExpiresAt");
        migrationBuilder.CreateIndex("IX_JwtSessions_UserId", "JwtSessions", "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("JwtSessions");
        migrationBuilder.DropIndex("IX_Roles_Title", "Roles");
        migrationBuilder.DropIndex("IX_Users_NickName", "Users");
        migrationBuilder.DropForeignKey("FK_Users_Roles_RoleId", "Users");
        migrationBuilder.AlterColumn<int>("RoleId", "Users", type: "integer", nullable: true,
            oldClrType: typeof(int), oldType: "integer");
        migrationBuilder.AddForeignKey("FK_Users_Roles_RoleId", "Users", "RoleId", "Roles", principalColumn: "Id");
        migrationBuilder.DropForeignKey("FK_Tokens_Users_UserId", "Tokens");
        migrationBuilder.AddForeignKey("FK_Tokens_Users_UserId", "Tokens", "UserId", "Users", principalColumn: "Id");
    }
}
