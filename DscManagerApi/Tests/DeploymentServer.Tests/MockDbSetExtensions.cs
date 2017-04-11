using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentServer.Tests
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    using Moq;

    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> SetupData<T>(this Mock<DbSet<T>> mockSet, IEnumerable<T> sourceData) where T : class
        {
            var querySet = sourceData.AsQueryable();
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(querySet.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(querySet.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(querySet.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(querySet.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(querySet.GetEnumerator());
            mockSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockSet.Object);
            
            return mockSet;
        }
    }
}
