using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace AspNetCore.Identity.Neo4j.Internal.Extensions
{
    internal static class StatementResultCursorExtensions
    {
        /// <summary>
        /// Return the only record in the result stream.
        /// </summary>
        /// <param name="result">The result stream</param>
        /// <returns>The single element of the input sequence, or null if the sequence contains no elements.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="result">source</paramref> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
        public static async Task<IRecord> SingleOrDefaultAsync(this IStatementResultCursor result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (!await result.FetchAsync().ConfigureAwait(false))
            {
                return null;
            }

            var record = result.Current;
            if (!await result.FetchAsync().ConfigureAwait(false))
            {
                return record;
            }

            throw new InvalidOperationException("The result contains more than one element.");
        }

        /// <summary>
        /// Return the only record in the result stream.
        /// </summary>
        /// <param name="result">The result stream</param>
        /// <typeparam name="TResult">The type of the returning value.</typeparam>
        /// <returns>The single element of the input sequence, or default(<paramref name="TResult">TResult</paramref> if the sequence contains no elements.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="result">result</paramref> is null.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="operation">operation</paramref> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The input sequence contains more than one element.</exception>
        public static async Task<TResult> SingleOrDefaultAsync<TResult>(this IStatementResultCursor result, Func<IRecord, TResult> operation)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (!await result.FetchAsync().ConfigureAwait(false))
            {
                return default(TResult);
            }

            var record = result.Current;
            if (!await result.FetchAsync().ConfigureAwait(false))
            {
                return operation(record);
            }

            throw new InvalidOperationException("The result contains more than one element.");
        }
    }
}
