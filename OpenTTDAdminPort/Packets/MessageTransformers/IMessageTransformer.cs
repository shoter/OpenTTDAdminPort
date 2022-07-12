using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    /// <summary>
    /// Transforms given <see cref="TAdminMessage"/> into packet.
    /// </summary>
    /// <typeparam name="TAdminMessage">The type of the admin message.</typeparam>
    internal interface IMessageTransformer
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        AdminMessageType SupportedMessageType { get; }

        /// <summary>
        /// Reads the admin message and transforms it into packet.
        /// </summary>
        /// <param name="message">The packet that can be transformed into <see cref="TAdminMessage"/></param>
        /// <returns>Transformed message</returns>
        Packet Transform(in IAdminMessage message);
    }

    /// <summary>
    /// Transforms given <see cref="TAdminMessage"/> into packet.
    /// </summary>
    /// <typeparam name="TAdminMessage">The type of the admin message.</typeparam>
    internal interface IMessageTransformer<TAdminMessage> : IMessageTransformer
        where TAdminMessage : IAdminMessage
    {
        /// <summary>
        /// Reads the admin message and transforms it into packet.
        /// </summary>
        /// <param name="message">The packet that can be transformed into <see cref="TAdminMessage"/></param>
        /// <returns>Transformed message</returns>
        sealed Packet Transform(in TAdminMessage message) => Transform((IAdminMessage)message);
    }
}
