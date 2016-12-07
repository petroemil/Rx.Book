using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace SearchExample.RxService
{
    public static class ServiceCallWrapper
    {
        private static readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(250);
        private static readonly int numberOfRetries = 3;

        public static IObservable<TOut> WrapServiceCall<TIn, TOut>(IObservable<TIn> source, Func<TIn, Task<TOut>> serviceCall)
        {
            return source
                .Throttle(throttleInterval)
                .DistinctUntilChanged()
                .Select(param => Observable
                    .FromAsync(() => serviceCall(param))
                    .Timeout(timeoutInterval)
                    .Retry(numberOfRetries))
                .Switch();
        }
    }

    public static class ServiceCallWrapperExtensions
    {
        public static IObservable<TOut> CallService<TIn, TOut>(this IObservable<TIn> source, Func<TIn, Task<TOut>> serviceCall)
        {
            return ServiceCallWrapper.WrapServiceCall(source, serviceCall);
        }
    }
}