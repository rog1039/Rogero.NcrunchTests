using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Xunit;

namespace Rogero.NcrunchTests.SharedState.Tests
{
    public class ContextSharedStateTests
    {
        private DatabaseContext _context;
        private int _onModelCreatingCallCount = 0;

        public ContextSharedStateTests()
        {
            Action action = () => _onModelCreatingCallCount += 1;
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase();
            _context = new DatabaseContext(action, optionsBuilder.Options);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Test1()
        {
            var entityACount = _context.EntityA.Count();
            Assert.Equal(1, _onModelCreatingCallCount);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void Test2()
        {
            var entityACount = _context.EntityA.Count();
            Assert.Equal(1, _onModelCreatingCallCount);
        }
    }
}
