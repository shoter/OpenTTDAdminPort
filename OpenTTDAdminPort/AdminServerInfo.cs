using OpenTTDAdminPort.Game;

namespace OpenTTDAdminPort;

public record AdminServerInfo(
    string ServerName,
    string RevisionName,
    bool IsDedicated,
    string MapName,
    OttdDate Date,
    Landscape Landscape,
    ushort MapWidth,
    ushort MapHeight
    );
