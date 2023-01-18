using OpenTTDAdminPort.MainActor;

namespace OpenTTDAdminPort.Messages;

internal record AdminPortClientStateChange(MainState PreviousState, MainState CurrentState);
