using System;
using System.Threading.Tasks;

namespace SearchExample.TraditionalService
{
    public class ServiceCallWrapper<TParam, TResult>
    {
        private readonly Func<TParam, Task<TResult>> wrappedServiceCall;
        public ServiceCallWrapper(Func<TParam, Task<TResult>> wrappedServiceCall)
        {
            this.wrappedServiceCall = wrappedServiceCall;
        }

        // Throttle global variables
        private readonly TimeSpan throttleInterval = TimeSpan.FromMilliseconds(500);
        private DateTime lastThrottledParameterDate;

        // Distinct global variables
        private TParam lastDistinctParameter;

        // Retry global variables 
        private readonly int numberOfRetries = 3;

        // Timeout global variables 
        private readonly TimeSpan timeoutInterval = TimeSpan.FromMilliseconds(500);

        // Switch global variables 
        private Task<TResult> lastCall;

        // Callback events 
        public event Action<TResult> CallBack;
        public event Action<Exception> ErrorCallBack;

        public async void ServiceCall(TParam query)
        {
            try
            {
                // Throttle logic
                this.lastThrottledParameterDate = DateTime.Now;
                await Task.Delay(this.throttleInterval);

                if (DateTime.Now - this.lastThrottledParameterDate < this.throttleInterval) return;

                // Distinct logic
                if (this.lastDistinctParameter?.Equals(query) != true)
                    this.lastDistinctParameter = query;
                else
                    return;

                var newCall = Task.Run(async () =>
                {
                    // Retry logic
                    for (var tries = 0; tries < this.numberOfRetries; tries++)
                    {
                        try
                        {
                            // Timeout logic
                            var newTask = this.wrappedServiceCall(query);

                            var timeoutTask = Task.Delay(this.timeoutInterval);

                            var firstTaskToEnd = await Task.WhenAny(newTask, timeoutTask);

                            if (newTask == firstTaskToEnd) return newTask.Result;
                            else throw new Exception("Timeout");
                        }
                        catch { /* deal with the exception */ }
                    }

                    throw new Exception("Out of retries");
                });

                // Switch logic
                this.lastCall = newCall;

                var result = await newCall;

                if (this.lastCall == newCall)
                    this.CallBack?.Invoke(result);
            }

            catch (Exception ex)
            {
                this.ErrorCallBack?.Invoke(ex);
            }
        }
    }
}
