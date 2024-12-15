set "targetProject=src/Everest.Targets/Everest.Targets.csproj" 

dotnet list %targetProject% package --include-transitive > dependencies.txt