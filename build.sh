#!/bin/bash

cd $(dirname $0)
source "$PWD/.env"

dotnet build ".\\$ID.csproj" -c Debug -nologo

mkdir "$PWD/workshop/Assemblies/dependencies/"

mv "$PWD/bin/Debug/net48/$ID.dll" "$PWD/publish/$ID.dll"
cp "$PWD/publish/$ID.dll" "$PWD/workshop/Assemblies"

rm "$PWD/publish/$ID.zip"

cd "$PWD/publish/"
zip -q -r "$PWD/../$ID.zip" .
cd "$PWD/../"

mv "$PWD/$ID.zip" "$PWD/publish/$ID.zip"
