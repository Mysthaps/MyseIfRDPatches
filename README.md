A plugin that adds several modifications to Rhythm Doctor.

## Features

- **CustomChiliSpeed**, **CustomIceSpeed**: Allows changing the chili/ice speeds to a value higher or lower than the game's defaults.
- **EnableBossSpeedChange**: Allows the speed of boss levels to be changed.
- **ShowFPS**: Adds an FPS counter on the top left of the screen while in a level.
- **FailCondition**: Fail the level if a condition is met.
- **GhostTapMiss**: Get a miss for pressing the button without hitting a beat.
- **ShowAccuracy**: Adds an accuracy count at the end of a level if Detailed Level Results is enabled.
- **AutoArtistLinks**: Automatically adds artist links to a level.


## Notes
- All configurations are disabled by default. Enable them via BepInEx Configuration Manager.
- For **CustomChiliSpeed**: Normal oneshot animations will be broken for higher speeds. Bars shorter than 200ms will not work properly.
- For **ShowAccuracy**: Accuracy chart is as below:

| Margins | Accuracy |
| --- | --- |
| ±25ms | +0.01% (for ADOFAI accuracy type) |
| ±40ms | 100% |
| ±80ms | 85% |
| ±120ms | 60% |
| ±400ms or completely miss | 0% |

## Installation
1. If this is your first time, download the Installation Quickstart zip from [here](https://github.com/Mysthaps/MyseIfRDPlugins/releases). Unzip it in your Rhythm Doctor installation folder. The zip file contains BepInEx 5, Configuration Manager and ScriptEngine, but not the mod itself.
2. Download the latest version of the mod from [here](https://github.com/Mysthaps/MyseIfRDPlugins/releases). It should be named `MyseIfRDPlugins_2.x.x.dll`. Put the file in `BepInEx/scripts/` in your Rhythm Doctor installation folder.
3. Launch the game.
    - If BepInEx does not load, change `ignoreDisableSwitch` to `true` in `doorstop_config.ini`.
4. Configure the mod in-game with a GUI by pressing `F1`. Reload the plugin and settings by pressing `F6`.

For more information, check out the [BepInEx installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html).
