using Akka.TestKit.Xunit2;

using OpenTTDAdminPort.Common;

using System;

namespace OpenTTDAdminPort.Tests
{
    public abstract class BaseTestKit : TestKit
    {

        public void WaitUntil(Func<bool> checkFunc)
        {
            if(TaskHelper.WaitUntil(checkFunc, TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(5)).Result)
            {
                return;
            }
            throw new Exception("Wait until timeout");

        }

        public void WaitUntil(Func<bool> checkFunc, float secondWaitingTime)
        {
            if (TaskHelper.WaitUntil(checkFunc, TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(secondWaitingTime)).Result)
            {
                return;
            }

            throw new Exception("Wait until timeout");
        }

    }
}
