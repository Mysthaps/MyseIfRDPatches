A plugin that adds several modifications to Rhythm Doctor.

## Features

- **ShowFPS**: Adds an FPS counter on the top left of the screen while in a level.
- **ShowAccuracy**: Adds an accuracy count at the end of a level if Detailed Level Results is enabled.
- **AutoArtistLinks**: Automatically adds artist links to a level. (from ADOFAI)
- **ChallengeMode**: Makes levels more challenging.
- **EnableOldSpeedChange**: Changes chili/ice speed to be 2x/.5x, respectively.
- **RankColorOnDoubleSpeed**: Changes rank colors if **EnableOldSpeedChange** is enabled.
- **EnableBossSpeedChange**: Allows the speed of boss levels to be changed.

## Notes
- All configurations are disabled by default. Enable them from `BepInEx/config/com.rhythmdr.garbageplugins.cfg` or via BepInEx Configuration Manager.
- For **ShowAccuracy**:
  - Does not work in 2P mode yet.
  - Calculating accuracy:

| Margins | Accuracy |
| --- | --- |
| ±25ms | +0.01% |
| ±40ms | 100% |
| ±80ms | 85% |
| ±120ms | 60% |
| ±400ms or completely miss | 0% |
- For **EnableOldSpeedChange**: Normal oneshot animations will be broken. Bars shorter than 200ms will not work properly.
- For **ChallengeMode**: There are two modes:
  - Heartbreak: Level fails if you crack any row's heart.
  - Perfect: Level fails if you get a single miss.

## Installation
1. Download the latest version of BepInEx 5 [here](https://github.com/BepInEx/BepInEx/releases). (Scroll down past the BepInEx 6 pre-release)
    - **Make sure you use the correct architecture for your system!**
      - If you are on a 32-bit version of Windows, select the x86 download.
      - If you are on a 64-bit version of Windows, select the x64 download. 
      - Otherwise, chose the unix download.
2. Unzip the file into your RD folder. You should have a `winhttp.dll`, `doorstop_config.ini`, and `BepInEx` folder next to Rhythm Doctor.exe.
3. Launch RD once to generate BepInEx files.
4. Download the latest version of the mod from [here](https://github.com/HellUser0/GarbagePlugins/releases). It should be named `GarbagePlugins_1.x.x.zip`.
5. Unzip the file you downloaded into your Rhythm Doctor installation folder. Put the file at `BepInEx/plugins/GarbagePlugins/GarbagePlugins.dll`.
6. Launch the game.
    - If BepInEx does not load, change `ignoreDisableSwitch` to `true` in `doorstop_config.ini`.
7. Configure the plugin as needed in `BepInEx/config/com.rhythmdr.garbageplugins.cfg`.
8. **Optional:** Install the [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) to configure the mod with a GUI by pressing `F1`.
9. **Optional:** To enable in-game reloading, install the [BepInEx ScriptEngine](https://github.com/BepInEx/BepInEx.Debug/releases/latest), create a `scripts` folder in the `BepInEx` folder then move the `GarbagePlugins.dll` file to that folder. You should now be able to reload the plugin by pressing `F6` in-game. Enabling "LoadOnStart" is highly recommended.

For more information, check out the [BepInEx installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html).
