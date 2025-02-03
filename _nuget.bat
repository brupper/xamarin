set setApiKey=
set version=0.9.1

dotnet nuget push .artifacts/Brupper.AspNetCore.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.AspNetCore.Breadcrumbs.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.AspNetCore.Identity.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.AspNetCore.Identity..B2C.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Core.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.Azure.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.EF.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.Sqlite.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Identity.B2C.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Jobs.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Jobs.FileTransfer.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Maui.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
rem dotnet nuget push .artifacts/Brupper.Push.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
rem dotnet nuget push .artifacts/Brupper.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
pause