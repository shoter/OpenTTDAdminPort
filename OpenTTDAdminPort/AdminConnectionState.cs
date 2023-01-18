using System;
using OpenTTDAdminPort.MainActor;

namespace OpenTTDAdminPort
{
    public enum AdminConnectionState
    {
        Idle,

        /// <summary>
        /// Establishing connection.
        /// Status will turn into <see cref="Connected"/>
        /// </summary>
        Connecting,

        /// <summary>
        /// Connection established
        /// </summary>
        Connected,

        Errored,
    }

    public static class AdminConnectionStateExtensions
    {
        public static AdminConnectionState ToConnectionState(this MainState state)
        {
            return state switch
            {
                MainState.Connected => AdminConnectionState.Connected,
                MainState.Idle => AdminConnectionState.Idle,
                MainState.Connecting => AdminConnectionState.Connecting,
                MainState.Errored => AdminConnectionState.Errored,
                _ => throw new ArgumentOutOfRangeException($"{state}");
            };
        }
    }
}
