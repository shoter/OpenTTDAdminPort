using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.States
{
    //public class AdminPortIdleState : IAdminPortClientState
    //{

    //    public async Task Connect(AdminPortClientContext context)
    //    {
    //        try
    //        {
    //            context.cancellationTokenSource = new CancellationTokenSource();

    //            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => MainLoop(cancellationTokenSource.Token)), null);
    //            ThreadPool.QueueUserWorkItem(new WaitCallback((_) => EventLoop(cancellationTokenSource.Token)), null);

    //            if (!(await TaskHelper.WaitUntil(() => context.State == AdminConnectionState.Connected, delayBetweenChecks: TimeSpan.FromSeconds(0.5), duration: TimeSpan.FromSeconds(10))))
    //            {
    //                this.cancellationTokenSource.Cancel();
    //                this.cancellationTokenSource = new CancellationTokenSource();
    //                throw new AdminPortException("Admin port could not connect to the server");
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            this.ConnectionState = AdminConnectionState.Idle;
    //            throw new AdminPortException("Could not join server", e);
    //        }
    //    }

    //    public Task Disconnect()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
