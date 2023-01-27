namespace OpenTTDAdminPort;

public record AdminConnectionStateChange(AdminConnectionState PreviousState, AdminConnectionState NewState);
