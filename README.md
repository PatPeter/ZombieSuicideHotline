# Zombie Suicide Hotline
This is a plugin for Exiled 2.0 that prevents SCP-049-2 from commiting suicide.

## Configuration Settings
Key | Value Type | Default Value | Description
--- | --- | --- | ---
is_enabled               | boolean    |  true | Is the plugin enabled?
allow_recall             | boolean    | false | Enable or disable SCP-049 using .recall to teleport zombies back.
allow_vent               | boolean    | false | Enable or disable SCP-173 ~~using .vent to teleport to another SCP~~ BEING A SUSSY BAKA.
allow_unstuck            | boolean    | false | Enable or disable command for when players get stuck in the map or outside of it.
respawn_zombie_ragequits | boolean    | false | Enable or disable respawning players who ragequit the game after being killed by SCP-049.
recall_cooldown          | float      |    20 | How many seconds between each use of .recall?
vent_cooldown            | float      |    40 | How many seconds between each use of .vent?
unstuck_time             | int        |   300 | How long does a player need to be in the same room in order to be considered stuck?
hotline_calls:           | Dictionary |       |  
  Scp049                 | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp0492                | float      |     0 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp096                 | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp106                 | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp173                 | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp93953               | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
  Scp93989               | float      |    -1 | -1 disabled, 0 teleports to spawn with no damage, > 0 do this % of damage to current health
