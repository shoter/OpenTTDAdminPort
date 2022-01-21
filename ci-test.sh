param(
  [Parameter(Position = 0)]
  [string]$codecovProfile = 'Release'
)

dotnet test --collect "XPlat Code Coverage" -c $codecovProfile