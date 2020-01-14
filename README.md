# ExileApi private fork of PoeHud

Dirty copy of my private fork.
Difference with main fork:
* Read memory  like structs (better for CPU, but used more memory)
* New cache system
* New rendering with DX11
* Plugins can compile from source
* A lot diagnostic information for easy found performance problem
* No hooks mouse and keyboard for prevent lag when debug
* All "standard" plugins cut. (Now they should be like another plugins)

# Requirements:
* .NET 4.8 installed https://dotnet.microsoft.com/download/thank-you/net48

# For developers:

Build requirements:
* Visual Studio 2019 (with 2017 could be a problems, at least need packages downgrade).
* .NET 4.8 Developer Pack should be installed https://dotnet.microsoft.com/download/thank-you/net48-developer-pack (don't click on languages/packages at the bottom of page, this is language packages, not the main .NET 4.8 DevPack!).

Compilation:
* Create a new folder (HUD for example)
* Download release version from current repo, put to HUD directory (HUD\PoeHelper)
* Clone the this repo code to HUD folder (HUD\ExileApi)
* Open&Build ExileApi\ExileApi.sln solution.
* Program will be automatically copied to PoeHelper directory.

Known build errors:
* (check project references) References error in some project: unload then load project to solution (right mouse button on project in solution).
* Error with MsBuildMajorVersion: Update your VS2019 (maybe you have VS 2019 Preview or something)
* MsBuild 15 < 16 error: expecting VS 2019 installed. On VS 2017 try downgrade Fody package to version 4.2.1.

## Troubleshooting

* Download problems:

When download your `7z` from releases maybe this comes with screwed permissions.

> For those who can't launch (Close as soon as it's opened) :
> Right click on the Zip of the HUD (The one you get from the link in the first post) and Right click > Properties > Unlock, then you can unzip it where you want. Otherwise all the files extracted will be security locked...
> Worked for a friend, it's a security from Windows that deny access to files from another PC, once done everything was good for him

* Rendering problems

Big visual offsets in rendering minion dots and everything other:

Windows Display options-> Scale and layout -> set to 100%
