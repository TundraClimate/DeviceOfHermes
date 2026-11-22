#!/bin/bash

cd $(dirname $0)
source "$PWD/.env"

dotnet build -c Debug -nologo

cargo build -r --target x86_64-pc-windows-gnu

if [ ! -d "$PWD/workshop/Assemblies/dependencies" ]; then
	mkdir "$PWD/workshop/Assemblies/dependencies/"
fi

if [ ! -d "$PWD/workshop/Assemblies/dependencies_native" ]; then
	mkdir "$PWD/workshop/Assemblies/dependencies_native/"
fi

mv "$PWD/src/Core/bin/Debug/net48/$ID.dll" "$PWD/publish/$ID.dll"
cp "$PWD/publish/$ID.dll" "$PWD/workshop/Assemblies"

mv "$PWD/src/LimbufOfHermes/bin/Debug/net48/LimbufOfHermes.dll" "$PWD/publish/LimbufOfHermes.dll"
cp "$PWD/publish/LimbufOfHermes.dll" "$PWD/workshop/Assemblies/HermesAssemblies"

mv "$PWD/src/ScrapOfHermes/bin/Debug/net48/ScrapOfHermes.dll" "$PWD/publish/ScrapOfHermes.dll"
cp "$PWD/publish/ScrapOfHermes.dll" "$PWD/workshop/Assemblies/HermesAssemblies"

mv "$PWD/src/Caduceus/bin/Debug/netstandard2.0/Caduceus.dll" "$PWD/publish/Caduceus.dll"
cp "$PWD/publish/Caduceus.dll" "$PWD/workshop/"

mv "$PWD/target/x86_64-pc-windows-gnu/release/liqtal_ffi.dll" "$PWD/publish/liqtal_ffi.dll"
cp "$PWD/publish/liqtal_ffi.dll" "$PWD/workshop/Assemblies/dependencies_native/"

rm "$PWD/publish/$ID.zip"

cd "$PWD/publish/"
zip -q -r "$PWD/../$ID.zip" .
cd "$PWD/../"

mv "$PWD/$ID.zip" "$PWD/publish/$ID.zip"
