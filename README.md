# Zombie Suicide Hotline
A plugin for SCP: Secret Laboratory using NWAPI that prevents SCP-049-2 (zombies) from committing suicide and adds utility commands for SCPs.

## Features
- Prevents SCP-049-2 from suiciding.
- Adds commands for SCPs, such as `.recall`, `.vent`, `.unstuck`, and perks.
- Optional respawn for players who disconnect after being killed by SCP-049.
- Configurable cooldowns and behaviors for each command.

## Configuration Settings

| Key                      | Value Type | Default Value | Description                                                                                   |
|--------------------------|------------|---------------|-----------------------------------------------------------------------------------------------|
| is_enabled               | boolean    | true          | Enables or disables the plugin.                                                               |
| allow_recall             | boolean    | false         | Allows SCP-049 to use `.recall` to teleport zombies back.                                     |
| allow_vent               | boolean    | false         | Allows SCP-173 to use `.vent` to teleport.                                                    |
| allow_unstuck            | boolean    | false         | Enables the `.unstuck` command for players stuck in or outside the map.                       |
| respawn_zombie_ragequits | boolean    | false         | Respawns players who disconnect after being killed by SCP-049.                                |
| recall_cooldown          | float      | 20            | Cooldown in seconds between uses of `.recall`.                                                |
| vent_cooldown            | float      | 40            | Cooldown in seconds between uses of `.vent`.                                                  |
| unstuck_time             | int        | 300           | Time (in seconds) a player must be in the same room to be considered stuck.                   |
| hotline_calls            | Dictionary |               | Per-SCP settings for the hotline call feature (see below).                                    |

### hotline_calls Dictionary

| SCP        | float | Default | Description                                                                                   |
|------------|-------|---------|-----------------------------------------------------------------------------------------------|
| Scp049     | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp0492    | 0     | 0       | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp096     | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp106     | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp173     | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp93953   | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |
| Scp93989   | -1    | -1      | -1: disabled, 0: teleport to spawn with no damage, >0: deal % of current health as damage      |

## Requirements
- NWAPI (Northwood API) for SCP: Secret Laboratory
- .NET Framework 4.8

## Installation
1. Place the compiled DLL in your NWAPI plugins folder.
2. Configure `ZombieSuicideHotline.yml` as needed.
3. Restart your server.

## Commands
- `.recall` — Teleports SCP-049-2s back to SCP-049 (if enabled).
- `.vent` — Allows SCP-173 to teleport (if enabled).
- `.unstuck` — Allows players to teleport to spawn if stuck (if enabled).
- `.perks` — Lists available perks and allows activation (if enabled).
    - `.metzitzahbpeh` — As SCP-049, circumcise your zombies and drain their blood to gain health. Damages all your SCP-049-2s and heals you for a percentage of their current health (if enabled).
    - `.passover` — As SCP-049, sacrifice your "firstborn" zombie and gain its full health as healing. Instantly kills your first SCP-049-2 and heals you for its current health (if enabled).

## Notes
- This plugin is for NWAPI, not Exiled.
- Some features require enabling in the configuration file.
