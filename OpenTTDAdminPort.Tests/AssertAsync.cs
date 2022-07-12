using System;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Tests
{
    public static class AssertAsync
    {
        public static void AssertCompletesIn(this Action action, TimeSpan timeSpan)
        {
            var task = Task.Run(action);
            var completedInTime = Task.WaitAll(new[] { task }, timeSpan);

            if (!completedInTime)
            {
                throw new TimeoutException($"Task did not complete in {timeSpan.TotalSeconds} seconds.");
            }

            if (task.Exception != null)
            {
                if (task.Exception.InnerExceptions.Count == 1)
                {
                    throw task.Exception.InnerExceptions[0];
                }

                throw task.Exception;
            }
        }
    }
}
