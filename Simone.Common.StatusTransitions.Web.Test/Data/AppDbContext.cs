using Microsoft.EntityFrameworkCore;
using Simone.Common.StatusTransitions.Web.Test.Models;

namespace Simone.Common.StatusTransitions.Web.Test.Data
{
   /// <summary>
   /// Application DbContext for the application
   /// </summary>
   /// <param name="options"></param>
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {

        // ---- Business table ----
        public DbSet<Subcontract> Subcontracts { get; set; }
    }
}
