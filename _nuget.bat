set setApiKey=oy2d7p42aldxblkvxwuqfed7x342x7te3h4vpc2ynrdmva
set version=0.0.26

dotnet nuget push .artifacts/Brupper.Core.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.Azure.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.EF.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Data.Sqlite%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Forms.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Forms.FontAwesome.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Identity.B2C%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Identity.B2C.Admin.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
dotnet nuget push .artifacts/Brupper.Push.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
rem dotnet nuget push .artifacts/Brupper.%version%.nupkg --api-key %setApiKey% --source https://api.nuget.org/v3/index.json --skip-duplicate
pause