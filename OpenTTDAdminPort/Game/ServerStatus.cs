using System.Collections.Generic;

namespace OpenTTDAdminPort.Game;

public record ServerStatus(AdminServerInfo AdminServerInfo, Dictionary<uint, Player> Players);
