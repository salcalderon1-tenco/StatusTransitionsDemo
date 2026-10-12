using Microsoft.EntityFrameworkCore;
using Simone.Common.StatusTransitions.Abstractions;
using Simone.Common.StatusTransitions.Models;
using Simone.Common.StatusTransitions.Web.Test.Models;

namespace Simone.Common.StatusTransitions.Web.Test.Data
{
   /// <summary>
   /// Application DbContext for the application
   /// </summary>
   /// <param name="options"></param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IStatusTransitionContext
    {

        // ---- Business table ----
        public DbSet<Subcontract> Subcontracts { get; set; }

        // ---- Tables required by the package ----
        public DbSet<Process> Processes { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<StatusRelation> StatusRelations { get; set; }
        public DbSet<StatusRelationPermission> StatusRelationPermissions { get; set; }
        public DbSet<BaseTransition> Transitions { get; set; }
        public DbSet<NotificationType> NotificationTypes { get; set; }
    }
}
