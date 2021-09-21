namespace ToDoApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Directory",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    DirectoryName = c.String(maxLength: 1000, unicode: false),
                    User_Id = c.String(maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    Email = c.String(maxLength: 256, unicode: false),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(maxLength: 1000, unicode: false),
                    SecurityStamp = c.String(maxLength: 1000, unicode: false),
                    PhoneNumber = c.String(maxLength: 1000, unicode: false),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(precision: 0),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                    ClaimType = c.String(maxLength: 1000, unicode: false),
                    ClaimValue = c.String(maxLength: 1000, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128, unicode: false),
                    ProviderKey = c.String(nullable: false, maxLength: 128, unicode: false),
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128, unicode: false),
                    RoleId = c.String(nullable: false, maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128, unicode: false),
                    Name = c.String(nullable: false, maxLength: 256, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.ToDoTask",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(maxLength: 1000, unicode: false),
                    Header = c.String(maxLength: 1000, unicode: false),
                    IsFavorite = c.Boolean(nullable: false),
                    IsDone = c.Boolean(nullable: false),
                    IsOverdue = c.Boolean(nullable: false),
                    DateOfTask = c.DateTime(precision: 0),
                    Directory_Id = c.Int(),
                    User_Id = c.String(maxLength: 128, unicode: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directory", t => t.Directory_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.Directory_Id)
                .Index(t => t.User_Id);

            CreateStoredProcedure(
                "dbo.Directory_Insert",
                p => new
                {
                    DirectoryName = p.String(maxLength: 1000, unicode: false),
                    User_Id = p.String(maxLength: 128, unicode: false, storeType: "nvarchar"),
                },
                body:
                    @"SET SESSION sql_mode='ANSI';INSERT INTO `Directory`(
                      `DirectoryName`, 
                      `User_Id`) VALUES (
                      @DirectoryName, 
                      @User_Id);
                      SELECT
                      `Id`
                      FROM `Directory`
                       WHERE  row_count() > 0 AND `Id`=last_insert_id();"
            );

            CreateStoredProcedure(
                "dbo.Directory_Update",
                p => new
                {
                    Id = p.Int(),
                    DirectoryName = p.String(maxLength: 1000, unicode: false),
                    User_Id = p.String(maxLength: 128, unicode: false, storeType: "nvarchar"),
                },
                body:
                    @"UPDATE `Directory` SET `DirectoryName`= DirectoryName, `User_Id`=User_Id WHERE todotask.Id = Id;"
            );

            CreateStoredProcedure(
                "dbo.Directory_Delete",
                p => new
                {
                    Id = p.Int(),
                    User_Id = p.String(maxLength: 128, unicode: false, storeType: "nvarchar"),
                },
                body:
                    @"UPDATE todotask SET todotask.Directory_Id = NULL WHERE todotask.Directory_Id = Id; DELETE FROM `Directory` WHERE `Directory`.Id = Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_Insert",
                p => new
                {
                    Description = p.String(maxLength: 1000, unicode: false),
                    Header = p.String(maxLength: 1000, unicode: false),
                    IsFavorite = p.Boolean(),
                    IsDone = p.Boolean(),
                    IsOverdue = p.Boolean(),
                    DateOfTask = p.DateTime(),
                    Directory_Id = p.Int(),
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"SET SESSION sql_mode='ANSI';INSERT INTO `ToDoTask`(
                      `Description`, 
                      `Header`, 
                      `IsFavorite`, 
                      `IsDone`, 
                      `IsOverdue`, 
                      `DateOfTask`, 
                      `Directory_Id`, 
                      `User_Id`) VALUES (
                      Description, 
                      Header, 
                      IsFavorite, 
                      IsDone, 
                      IsOverdue, 
                      DateOfTask, 
                      Directory_Id, 
                      User_Id);
                      SELECT
                      `Id`
                      FROM `ToDoTask`
                       WHERE  row_count() > 0 AND `Id`=last_insert_id();
                       Call ToDoTask_GetOverdueList(User_Id);"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_Update",
                p => new
                {
                    Id = p.Int(),
                    Description = p.String(maxLength: 1000, unicode: false),
                    Header = p.String(maxLength: 1000, unicode: false),
                    IsFavorite = p.Boolean(),
                    IsDone = p.Boolean(),
                    IsOverdue = p.Boolean(),
                    DateOfTask = p.DateTime(),
                    Directory_Id = p.Int(),
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"UPDATE `ToDoTask` SET `Description`=Description, `Header`=Header, `IsFavorite`=IsFavorite, `IsDone`=IsDone, `IsOverdue`=IsOverdue, `DateOfTask`=DateOfTask, `Directory_Id`=Directory_Id, `User_Id`=User_Id WHERE todotask.Id = Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_Delete",
                p => new
                {
                    Id = p.Int(),
                    Directory_Id = p.Int(),
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"DELETE FROM `ToDoTask` WHERE todotask.Id = Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_GetCompletedList",
                p => new
                {
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"SELECT * FROM todotask WHERE todotask.IsDone = 1 AND todotask.User_Id = User_Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_GetFavoriteList",
                p => new
                {
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"SELECT * FROM todotask WHERE todotask.IsFavorite LIKE (1) AND todotask.User_Id = User_Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_GetOverdueList",
                p => new
                {
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"UPDATE todotask SET todotask.IsOverdue  = 1 WHERE todotask.IsDone LIKE (0) AND Now() > todotask.DateOfTask AND todotask.User_Id = User_Id AND todotask.DateOfTask IS NOT NULL; SELECT * FROM todotask WHERE todotask.IsOverdue = 1 AND todotask.User_Id = User_Id;"
            );

            CreateStoredProcedure(
                "dbo.ToDoTask_TasksInDirectory",
                p => new
                {
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"SELECT * FROM todotask INNER JOIN todoapp.`directory` AS dr ON dr.Id = todotask.Directory_Id WHERE dr.User_Id = User_Id AND dr.Id = Directory_Id;"
            );


            CreateStoredProcedure(
                "dbo.ToDoTask_GetActiveList",
                p => new
                {
                    User_Id = p.String(maxLength: 128, unicode: false),
                },
                body:
                    @"UPDATE todotask SET todotask.IsOverdue = 0 WHERE todotask.IsDone LIKE (0) AND NOW() < todotask.DateOfTask AND todotask.User_Id = User_Id AND todotask.DateOfTask IS NOT NULL; SELECT * FROM todotask WHERE todotask.User_Id = User_Id;"
            );


        }

        public override void Down()
        {
            DropStoredProcedure("dbo.ToDoTask_Delete");
            DropStoredProcedure("dbo.ToDoTask_Update");
            DropStoredProcedure("dbo.ToDoTask_Insert");
            DropStoredProcedure("dbo.Directory_Delete");
            DropStoredProcedure("dbo.Directory_Update");
            DropStoredProcedure("dbo.Directory_Insert");
            DropForeignKey("dbo.ToDoTask", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ToDoTask", "Directory_Id", "dbo.Directory");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Directory", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ToDoTask", new[] { "User_Id" });
            DropIndex("dbo.ToDoTask", new[] { "Directory_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Directory", new[] { "User_Id" });
            DropTable("dbo.ToDoTask");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Directory");
        }
    }
}
