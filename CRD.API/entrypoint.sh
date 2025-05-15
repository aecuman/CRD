#!/bin/bash
set -e
dotnet ef database update --project CRD.Persistence/CRD.Persistence.csproj
dotnet run seeddata
exec "$@"
