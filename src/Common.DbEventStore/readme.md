# dotnet new classlib -n Play.Common  

## Command palette: .NET: Generate Assets for Build and Debug

 ```"group": {
        "kind": "build",
        "isDefault": true
      }
```

## Packages

```bash
dotnet add package MongoDb.Driver
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Binder
dotnet add package Microsoft.Extensions.DependencyInjection

dotnet nuget add source /Users/davidyaari/projects/Csharp/microservices/packages -n PlayEconomy
dotnet nuget remove source "PlayEconomy"
## In the src/Play.Common dir
##(before remomber to update the version on the .csproj)
dotnet pack -o ../../../packages     

```
