#!/bin/sh
set -e
dotnet ef database update
dotnet run seeddata
exec "$@"