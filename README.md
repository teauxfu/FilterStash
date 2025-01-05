# FilterStash

This is a prototype desktop utility for syncing PoE2 filter packages.

![demo](./docs/v0.1.3_example.png)

## Developing

The app targets .NET 9 on Windows 10+ only. The UI is implemented in Blazor and deployed through WinForms desktop shell. 
The installer is packaged using an Inno setup script. Releases are hosted on GitHub.

A separate MAUI shell is available, but is rather larger than the WinForms shell and doesn't offer any benefits over WinForms currently.

![app structure](./docs/app-structure.png)
### Todos
- winforms DI https://www.wiktorzychla.com/2022/01/winforms-dependency-injection-in-net6.html#google_vignette

## Changelog 
 
### v0.1.5 2025-01-05
added support for installing from zip files

### v0.1.4 2025-01-04
initial prototype release 

### prototype
- 2025-01-03 compressed wallpapers, added crude-auto-update
- 2025-01-03 packaging/deployment
- 2024-12-31 playing with ui, added native Windows menu, considering if MudBlazor is needed
	- still deliberating on what to do for data persistence. either json or sqlite via efcore
	- stealing some graphics from https://www.cameronsworld.net/
- 2024-12-30 toying with deployment methods, settled on MAUI Blazor with installs via InnoSetup
- 2024-12-29 initial scaffolding, connecting Octokit, etc.