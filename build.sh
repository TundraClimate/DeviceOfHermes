#!/bin/bash

cd $(dirname $0)
source "$PWD/.env"

dotnet build -c Debug -nologo

if [ ! -d "$PWD/workshop/Assemblies/dependencies" ]; then
	mkdir "$PWD/workshop/Assemblies/dependencies/"
fi

mv "$PWD/src/Core/bin/Debug/net48/$ID.dll" "$PWD/publish/$ID.dll"
cp "$PWD/publish/$ID.dll" "$PWD/workshop/Assemblies"

mv "$PWD/src/LimbufOfHermes/bin/Debug/net48/LimbufOfHermes.dll" "$PWD/publish/LimbufOfHermes.dll"
cp "$PWD/publish/LimbufOfHermes.dll" "$PWD/workshop/Assemblies/HermesAssemblies"

mv "$PWD/src/Caduceus/bin/Debug/netstandard2.0/Caduceus.dll" "$PWD/publish/Caduceus.dll"
cp "$PWD/publish/Caduceus.dll" "$PWD/workshop/"

rm "$PWD/publish/$ID.zip"

cd "$PWD/publish/"
zip -q -r "$PWD/../$ID.zip" .
cd "$PWD/../"

mv "$PWD/$ID.zip" "$PWD/publish/$ID.zip"
