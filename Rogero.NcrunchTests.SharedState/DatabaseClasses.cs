using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace Rogero.NcrunchTests.SharedState
{
    /// <summary>
    /// 
    /// A DbContext for interacting with the EntityA type. I do not actually use a 
    /// real database in these tests. The DbContextOptions parameter to the constructor
    /// is used to specify that we wil be using an InMemory "database" rather than
    /// an actual SQL Server-backed database.
    /// 
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Action to call when the OnModelCreating method is run.
        /// </summary>
        private readonly Action _action;

        public DatabaseContext(Action action, DbContextOptions options) : base(options)
        {
            _action = action;
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _action();
        }

        public DbSet<EntityA> EntityA { get; set; }
    }

    /// <summary>
    /// The simplest entity type possible to use in the DatabaseContext class.
    /// </summary>
    public class EntityA
    {
        public int Id { get; set; }
    }
}
