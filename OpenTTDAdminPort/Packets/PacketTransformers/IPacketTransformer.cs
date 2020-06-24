using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    /// <summary>
    /// Used to transform packet into specified <see cref="TAdminMessage"/>.
    /// </summary>
    /// <typeparam name="TAdminMessage">The type of the admin message.</typeparam>
    internal interface IPacketTransformer
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        AdminMessageType SupportedMessageType { get; }

        /// <summary>
        /// Reads the packet and transforms it into admin message.
        /// </summary>
        /// <param name="packet">The packet that can be transformed into <see cref="TAdminMessage"/></param>
        /// <returns>Transformed message</returns>
        IAdminMessage Transform(Packet packet);

    }

    /// <summary>
    /// Used to transform packet into specified <see cref="TAdminMessage"/>.
    /// </summary>
    /// <typeparam name="TAdminMessage">The type of the admin message.</typeparam>
    internal interface IPacketTransformer<TAdminMessage> : IPacketTransformer
        where TAdminMessage : IAdminMessage
    {
        /// <summary>
        /// Reads the packet and transforms it into admin message.
        /// </summary>
        /// <param name="packet">The packet that can be transformed into <see cref="TAdminMessage"/></param>
        /// <returns>Transformed message in proper type.</returns>
        public sealed TAdminMessage TransformTyped(Packet packet) => (TAdminMessage)Transform(packet);
    }
}
