using System.Collections.Generic;
using AutoFixture;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.MainActor.StateData;

namespace OpenTTDAdminPort.Tests.Cusomizations
{
    public class StateCusomizations : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ConnectedData>(
                c => c
                    .FromFactory(
                        () =>
                        {
                            return new ConnectedData(
                                null!,
                                fixture.Create<ServerInfo>(),
                                fixture.Create<string>(),
                                null,
                                new Dictionary<AdminUpdateType, AdminUpdateSetting>(),
                                fixture.Create<AdminServerInfo>(),
                                new Dictionary<uint, Player>(),
                                new Dictionary<byte, Company>(),
                                new Dictionary<uint, Player>());
                        })
                    .OmitAutoProperties());
        }
    }
}