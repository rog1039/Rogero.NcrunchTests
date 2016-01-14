using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Rogero.NcrunchTests.SharedState.Tests.NCrunch.Framework;
using Xunit;

namespace Rogero.NcrunchTests.SharedState.Tests
{
    public class ContextSharedStateTests
    {
        private DatabaseContext _context;
        private int _onModelCreatingCallCount = 0;
        private Action _action;
        private DbContextOptions _options;

        private static ConcurrentQueue<string> _consoleMessages = new ConcurrentQueue<string>(); 

        public ContextSharedStateTests()
        {
            PrintProcessAndThreadInfo();
            _action = () => _onModelCreatingCallCount += 1;
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseInMemoryDatabase();
            _options = optionsBuilder.Options;

            _context = new DatabaseContext(_action, _options);


            Thread.Sleep(1000);
            PrintProcessAndThreadInfo();
            Thread.Sleep(1000);
            PrintProcessAndThreadInfo();
            Thread.Sleep(1000);
        }


        /// <summary>
        /// 
        /// OnModelCreatingShouldBeCalledA and OnModelCreatingShouldBeCalledB
        /// are identical and both use a DbContext class that is created in the 
        /// constructor above.
        /// 
        /// I expect that each test would run in total isolation and therefore 
        /// each test would result in the OnModelCreating method running. If the
        /// tests are run one-at-a-time through the NCrunch Tests window in VS2015
        /// then this holds true and each test will pass.
        /// 
        /// However, often when the tests are run close together, such as when I 
        /// highlight both tests in the NCrunch Tests window in VS2015 and then
        /// run the tests, often (but not always), one of the two tests will fail
        /// stating that the _onModelCreatingCallCount is still 0 which means the
        /// OnModelCreating method was not called. The only way I believe this can
        /// happen is if the tests are not totally isolated. Perhaps they are 
        /// executed using the same process or AppDomain??
        /// 
        /// </summary>
        [Fact()]
        [Trait("Category", "Instant")]
        [Isolated]
        public void OnModelCreatingShouldBeCalledA()
        {
            _onModelCreatingCallCount = 0;
            var entityACount = _context.EntityA.Count();
            Assert.Equal(1, _onModelCreatingCallCount);
        }
        
        [Fact()]
        [Trait("Category", "Instant")]
        [Isolated]
        public void OnModelCreatingShouldBeCalledB()
        {
            _onModelCreatingCallCount = 0;
            var entityACount = _context.EntityA.Count();
            Assert.Equal(1, _onModelCreatingCallCount);
        }

        /// <summary>
        /// It appears that DbContext's somehow cache the model that
        /// is created during the OnModelCreating override method so subsequent
        /// creations of the same DbContext class do not cause the 
        /// OnModelCreating method to be called. It will only be called 
        /// the first time. This is shown in the test below.
        /// </summary>
        [Fact()]
        [Trait("Category", "Instant")]
        [Isolated]
        public void OnModelCreatingShouldOnlyBeCalledOnce()
        {
            _onModelCreatingCallCount = 0;

            _context = new DatabaseContext(_action, _options);
            var entityACount1 = _context.EntityA.Count();
            Assert.Equal(1, _onModelCreatingCallCount);

            _context = new DatabaseContext(_action, _options);
            var entityACount2 = _context.EntityA.Count();

            _context = new DatabaseContext(_action, _options);
            var entityACount3 = _context.EntityA.Count();

            Assert.Equal(1, _onModelCreatingCallCount);
        }

        private void PrintProcessAndThreadInfo()
        {
            var message = $"[{DateTime.Now:hh:mm:ss.ffff}] PID: {Process.GetCurrentProcess().Id,4}, ThreadId: {Thread.CurrentThread.ManagedThreadId,-2}";
            _consoleMessages.Enqueue(message);
            Console.WriteLine(message);
        }
    }

    namespace NCrunch.Framework
    {
        public class IsolatedAttribute : Attribute
        {
        }
    }
}
