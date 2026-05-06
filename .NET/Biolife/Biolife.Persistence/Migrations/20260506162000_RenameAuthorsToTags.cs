using Biolife.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biolife.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260506162000_RenameAuthorsToTags")]
    public partial class RenameAuthorsToTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
IF OBJECT_ID(N'[FK_Products_Authors_AuthorId]', N'F') IS NOT NULL
    ALTER TABLE [Products] DROP CONSTRAINT [FK_Products_Authors_AuthorId];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_AuthorId' AND object_id = OBJECT_ID(N'[Products]'))
    EXEC sp_rename N'[Products].[IX_Products_AuthorId]', N'IX_Products_TagId', N'INDEX';

IF COL_LENGTH(N'[Products]', N'AuthorId') IS NOT NULL AND COL_LENGTH(N'[Products]', N'TagId') IS NULL
    EXEC sp_rename N'[Products].[AuthorId]', N'TagId', N'COLUMN';

IF OBJECT_ID(N'[Authors]', N'U') IS NOT NULL AND OBJECT_ID(N'[Tags]', N'U') IS NULL
    EXEC sp_rename N'[Authors]', N'Tags';

IF COL_LENGTH(N'[Roles]', N'Author') IS NOT NULL AND COL_LENGTH(N'[Roles]', N'Tag') IS NULL
    EXEC sp_rename N'[Roles].[Author]', N'Tag', N'COLUMN';

IF OBJECT_ID(N'[FK_Products_Tags_TagId]', N'F') IS NULL
   AND OBJECT_ID(N'[Tags]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[Products]', N'TagId') IS NOT NULL
    ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Tags_TagId]
        FOREIGN KEY ([TagId]) REFERENCES [Tags] ([Id]) ON DELETE NO ACTION;
""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
IF OBJECT_ID(N'[FK_Products_Tags_TagId]', N'F') IS NOT NULL
    ALTER TABLE [Products] DROP CONSTRAINT [FK_Products_Tags_TagId];

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_TagId' AND object_id = OBJECT_ID(N'[Products]'))
    EXEC sp_rename N'[Products].[IX_Products_TagId]', N'IX_Products_AuthorId', N'INDEX';

IF COL_LENGTH(N'[Products]', N'TagId') IS NOT NULL AND COL_LENGTH(N'[Products]', N'AuthorId') IS NULL
    EXEC sp_rename N'[Products].[TagId]', N'AuthorId', N'COLUMN';

IF OBJECT_ID(N'[Tags]', N'U') IS NOT NULL AND OBJECT_ID(N'[Authors]', N'U') IS NULL
    EXEC sp_rename N'[Tags]', N'Authors';

IF COL_LENGTH(N'[Roles]', N'Tag') IS NOT NULL AND COL_LENGTH(N'[Roles]', N'Author') IS NULL
    EXEC sp_rename N'[Roles].[Tag]', N'Author', N'COLUMN';

IF OBJECT_ID(N'[FK_Products_Authors_AuthorId]', N'F') IS NULL
   AND OBJECT_ID(N'[Authors]', N'U') IS NOT NULL
   AND COL_LENGTH(N'[Products]', N'AuthorId') IS NOT NULL
    ALTER TABLE [Products] ADD CONSTRAINT [FK_Products_Authors_AuthorId]
        FOREIGN KEY ([AuthorId]) REFERENCES [Authors] ([Id]) ON DELETE NO ACTION;
""");
        }
    }
}
