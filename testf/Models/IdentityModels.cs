using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations.History;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;

namespace ToDoApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }


    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.ValidateOnSaveEnabled = false;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoTask>()
            .MapToStoredProcedures();
            modelBuilder.Entity<Directory>()
            .MapToStoredProcedures();

            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
            modelBuilder.Properties().Where(x =>
                    x.PropertyType.FullName != null &&
                    x.PropertyType.FullName.Equals("System.String") &&
                    !x.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(q => q.TypeName != null &&
                    q.TypeName.Equals("varchar(max)", StringComparison.InvariantCultureIgnoreCase))).Configure(c =>
                    c.HasColumnType("varchar(65000)"));

            modelBuilder.Properties().Where(x =>
                    x.PropertyType.FullName != null &&
                    x.PropertyType.FullName.Equals("System.String") &&
                    !x.GetCustomAttributes(false).OfType<ColumnAttribute>().Any(q => q.TypeName != null &&
                    q.TypeName.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase))).Configure(c =>
                    c.HasColumnType("varchar"));
        }
        public DbSet<ToDoTask> Tasks { get; set; }

        public DbSet<Directory> Directories { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}