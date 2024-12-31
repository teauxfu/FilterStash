# FilterStash

This is a prototype desktop utility for syncing PoE2 filter packages.

![app structure](./app-structure.png)

![general idea](./general-idea.png)

added inno setup for the installer
tried packaging in winforms to see if it would be easier, it wasn't -- static assets can't be served from an RCL in winforms apparently

## Todos

- ui lib, mudblazor or something?
- app db for storing hashes and such?
	- https://learn.microsoft.com/en-us/dotnet/maui/data-cloud/database-sqlite?view=net-maui-9.0
	- https://www.reddit.com/r/dotnetMAUI/comments/13j65uk/efcore_vs_sqlite_sqlitenetpcl/
	- https://www.reddit.com/r/dotnet/comments/vsjmjh/ef_core_with_sqlite_worth_the_effort/?rdt=62904
