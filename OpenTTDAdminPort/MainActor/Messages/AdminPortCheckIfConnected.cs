using System;

namespace OpenTTDAdminPort.MainActor.Messages;

/// <summary>
/// This message is used to check if AdminPortClient is not blocked for too long in connecting state.
/// </summary>
internal record AdminPortCheckIfConnected(Guid ConnectingId);
