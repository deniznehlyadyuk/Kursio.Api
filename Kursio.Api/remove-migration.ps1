$targetMigrationName = ($n = (Get-ChildItem "Migrations" | Sort-Object Name)[-5].BaseName.Split('_'))[1..($n.Length-1)] -join '_'
dotnet ef database update $targetMigrationName
dotnet ef migrations remove