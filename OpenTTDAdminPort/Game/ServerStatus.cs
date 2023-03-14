using System.Collections.Generic;

namespace OpenTTDAdminPort.Game;

public record ServerStatus(AdminServerInfo AdminServerInfo, IReadOnlyDictionary<uint, Player> Players);
