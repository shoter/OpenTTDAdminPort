param(
  [Parameter(Position = 0)]
  [string]$codecovProfile = 'Release'
)

dotnet test --collect "XPlat Code Coverage" --settings .\OpenTTDAdminPort.Tests\coverlet.runsettings -c $codecovProfile