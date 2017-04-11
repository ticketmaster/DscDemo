using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentServer.Tests
{
    using System.Data.Entity;
    
    using Moq;

    public static class DbContextExtensions
    {
        public static Mock<TContext> UseDbSet<TContext, T>(this Mock<TContext> mockContext, Mock<DbSet<T>> mockDbSet) where T : class where TContext : DbContext
        {
            mockContext.Setup(c => c.Set<T>()).Returns(mockDbSet.Object);
            return mockContext;
        }
    }
}
