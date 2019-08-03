using System;
using System.Threading;
using System.Threading.Tasks;
using Azuria.ErrorHandling;
using Azuria.Requests.Builder;

namespace Azuria.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IProxerResult> Invoke(IRequestBuilder request, Func<IRequestBuilder, Task<IProxerResult>> next,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IProxerResult<T>> InvokeWithResult<T>(IRequestBuilderWithResult<T> request,
            Func<IRequestBuilderWithResult<T>, Task<IProxerResult<T>>> next,
            CancellationToken cancellationToken = default);
    }
}