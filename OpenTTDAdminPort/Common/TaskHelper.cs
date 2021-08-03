using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Common
{
    internal static class TaskHelper
    {
        public static async Task<bool> WaitUntil(Func<bool> condition, TimeSpan delayBetweenChecks, TimeSpan duration)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime) < duration)
            {
                if (condition())
                    return true;

                await Task.Delay(delayBetweenChecks);
            }

            return false;
        }

        public static Task WaitMax(this Task task, [CallerMemberName] string? callerName = null) => task.WaitMax(TimeSpan.FromSeconds(10), callerName);
        public static async Task WaitMax(this Task task, TimeSpan waitTime, [CallerMemberName] string? callerName = null)
        {
            Task delayTask = Task.Delay(waitTime);

            await Task.WhenAny(task, delayTask);

            if (delayTask.IsCompleted)
            {
                throw new TaskWaitException($"Inside {callerName} there was task timeout. Refer to exception to find more details.");
            }
        }

        public static Task<T> WaitMax<T>(this Task<T> task, [CallerMemberName] string? callerName = null) => task.WaitMax(TimeSpan.FromSeconds(10), callerName);


        public static async Task<T> WaitMax<T>(this Task<T> task, TimeSpan waitTime, [CallerMemberName] string? callerName = null)
        {
            Task delayTask = Task.Delay(waitTime);

            await Task.WhenAny(task, delayTask);

            if (!task.IsCompleted)
            {
                throw new TaskWaitException($"Inside {callerName} there was task timeout. Refer to exception to find more details.");
            }

            return await task;
        }

        public static Task WaitWithToken(this Task task, CancellationToken token) => task.WaitWithToken(token, TimeSpan.FromMilliseconds(50));

        public static async Task WaitWithToken(this Task task, CancellationToken token, TimeSpan refreshDelay)
        {
            while (true)
            {
                await Task.Delay(refreshDelay);
                if (token.IsCancellationRequested)
                    break;
                if (task.IsCompleted)
                    break;
            }
        }

        public static Task<T> WaitWithToken<T>(this Task<T> task, CancellationToken token) => task.WaitWithToken(token, TimeSpan.FromMilliseconds(50));

        public static async Task<T> WaitWithToken<T>(this Task<T> task, CancellationToken token, TimeSpan refreshDelay)
        {
            while (true)
            {
                await Task.Delay(refreshDelay);
                if (token.IsCancellationRequested)
                {
#pragma warning disable CS8603 // Possible null reference return. (I am unable to declare return type as Task<T?> as it is not Net5. Also Nullable<T> would change semantics of returned value.
                    return default;
#pragma warning restore CS8603 // Possible null reference return.
                }
                if (task.IsCompleted)
                {
                    return task.Result;
                }
            }
        }
    }
}
