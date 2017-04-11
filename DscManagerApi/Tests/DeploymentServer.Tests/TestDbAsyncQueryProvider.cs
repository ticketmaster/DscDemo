// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDbAsyncQueryProvider.cs" company="Ticketmaster">
//   Copyright 2015 Ticketmaster
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DeploymentServer.Tests
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The test db async query provider.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        /// <summary>
        /// The _inner.
        /// </summary>
        private readonly IQueryProvider _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbAsyncQueryProvider{TEntity}"/> class.
        /// </summary>
        /// <param name="inner">
        /// The inner.
        /// </param>
        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            this._inner = inner;
        }

        /// <summary>
        /// The create query.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        /// <summary>
        /// The create query.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TElement">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQueryable"/>.
        /// </returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Execute(Expression expression)
        {
            return this._inner.Execute(expression);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TResult"/>.
        /// </returns>
        public TResult Execute<TResult>(Expression expression)
        {
            return this._inner.Execute<TResult>(expression);
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute(expression));
        }

        /// <summary>
        /// The execute async.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <typeparam name="TResult">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.Execute<TResult>(expression));
        }
    }

    /// <summary>
    /// The test db async enumerable.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbAsyncEnumerable{T}"/> class.
        /// </summary>
        /// <param name="enumerable">
        /// The enumerable.
        /// </param>
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbAsyncEnumerable{T}"/> class.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        /// <summary>
        /// Gets the provider.
        /// </summary>
        IQueryProvider IQueryable.Provider
        {
            get
            {
                return new TestDbAsyncQueryProvider<T>(this);
            }
        }

        /// <summary>
        /// The get async enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbAsyncEnumerator"/>.
        /// </returns>
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        /// <summary>
        /// The get async enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbAsyncEnumerator"/>.
        /// </returns>
        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }
    }

    /// <summary>
    /// The test db async enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        /// <summary>
        /// The _inner.
        /// </summary>
        private readonly IEnumerator<T> _inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbAsyncEnumerator{T}"/> class.
        /// </summary>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            this._inner = inner;
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        public T Current
        {
            get
            {
                return this._inner.Current;
            }
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        object IDbAsyncEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this._inner.Dispose();
        }

        /// <summary>
        /// The move next async.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this._inner.MoveNext());
        }
    }
}