﻿using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Events
{
    public interface IAdminMessageProcessor
    {
        IAdminEvent? ProcessMessage(IAdminMessage adminMessage, in IAdminPortClient client);
    }
}
