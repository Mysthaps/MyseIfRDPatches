A plugin that adds several modifications to Rhythm Doctor.

## Features

- **Custom Chili Speed**, **Custom Ice Speed**: Allows changing the chili/ice speeds to a value higher or lower than the game's defaults.
- **Enable Boss Speed Change**: Allows the speed of boss levels to be changed.
- **Show FPS**: Adds an FPS counter on the top left of the screen while in a level.
- **Fail Condition**: Fail the level if a condition is met.
- **Ghost Tap Miss**: Get a miss for pressing the button without hitting a beat.
- **Show Accuracy**: Adds an accuracy count at the end of a level if Detailed Level Results is enabled.
- **Auto Artist Links**: Automatically adds artist links to a level.
- **Pause Menu Transparency**: Changes the transparency of the *background* while paused.
- **Pause Menu Scale**: Changes the size of the menu while paused.


## Notes
- All configurations are disabled by default. Enable them via BepInEx Configuration Manager.
- For **CustomChiliSpeed**: Bars shorter than 200ms will not work properly.
- For **ShowAccuracy**: Accuracy chart is as below:

| Margins | Accuracy |
| --- | --- |
| ±25ms | +0.01% (for ADOFAI accuracy type) |
| ±40ms | 100% |
| ±80ms | 75% |
| ±120ms | 50% |
| ±400ms or completely miss | 0% |

## Installation
1. Download the latest version of BepInEx 5.
### Windows
You can download the Installation Quickstart zip from [the latest release](https://github.com/Mysthaps/MyseIfRDPatches/releases/latest/). Unzip it in your Rhythm Doctor installation folder. The zip file contains BepInEx 5, BepInEx Configuration Manager and ScriptEngine, but not the mod itself.
### Other
Follow the [installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html) for BepInEx5. Optionally, you can also download BepInEx Configuration Manager and ScriptEngine.

2. Download the latest version of the mod from the latest release. It should be named `MyseIfRDPlugins_2.x.x.dll`. Put the file in `BepInEx/scripts/` in your Rhythm Doctor installation folder. You'll need to redownload the file if you want to update the mod.
3. Launch the game.
    - If BepInEx does not load, change `ignoreDisableSwitch` to `true` in `doorstop_config.ini`.
4. With BepInEx Configuration Manager, configure the mod in-game with a GUI by pressing `F1`. With ScriptEngine, reload the plugin and settings by pressing `F6`.

For more information, check out the [BepInEx installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html).
