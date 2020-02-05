using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azuria.ErrorHandling;
using Azuria.Requests.Builder;

namespace Azuria.Middleware
{
    /// <summary>
    /// Adds some static headers to the request. They indicate things like application, version and api key used.
    /// </summary>
    public class StaticHeaderMiddleware : IMiddleware
    {
        private readonly IDictionary<string, string> _staticHeaders;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="staticHeaders"></param>
        public StaticHeaderMiddleware(IDictionary<string, string> staticHeaders)
        {
            this._staticHeaders = staticHeaders;
        }

        /// <inheritdoc />
        public Task<IProxerResult> Invoke(IRequestBuilder request, MiddlewareAction next,
            CancellationToken cancellationToken = default)
        {
            return next(request.WithHeader(this._staticHeaders), cancellationToken);
        }

        /// <inheritdoc />
        public Task<IProxerResult<T>> InvokeWithResult<T>(IRequestBuilderWithResult<T> request,
            MiddlewareAction<T> next, CancellationToken cancellationToken = default)
        {
            return next(request.WithHeader(this._staticHeaders), cancellationToken);
        }
    }
}