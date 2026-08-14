#!/bin/bash

cd $(dirname $0)
source "$PWD/.env"

dotnet build -c Debug -nologo

mkdir "$PWD/workshop/Assemblies/dependencies/"

mv "$PWD/Core/bin/Debug/net48/$ID.dll" "$PWD/publish/$ID.dll"
cp "$PWD/publish/$ID.dll" "$PWD/workshop/Assemblies"

mv "$PWD/LimbufOfHermes/bin/Debug/net48/LimbufOfHermes.dll" "$PWD/publish/LimbufOfHermes.dll"
cp "$PWD/publish/LimbufOfHermes.dll" "$PWD/workshop/Assemblies/HermesAssemblies"

rm "$PWD/publish/$ID.zip"

cd "$PWD/publish/"
zip -q -r "$PWD/../$ID.zip" .
cd "$PWD/../"

mv "$PWD/$ID.zip" "$PWD/publish/$ID.zip"
