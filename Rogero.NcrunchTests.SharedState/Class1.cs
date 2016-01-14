using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace Rogero.NcrunchTests.SharedState
{
    public class DatabaseContext : DbContext
    {
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

    public class EntityA
    {
        public int Id { get; set; }
    }
}
