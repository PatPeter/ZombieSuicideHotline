﻿namespace ZombieSuicideHotline
{
    using Exiled.Events.EventArgs;
    using Player = Exiled.API.Features.Player;
    using Exiled.API.Features;
    using System.Collections.Generic;

    public class PlayerHandlers
    {
        IDictionary<RoleType, UnityEngine.Vector3> Spawns = new Dictionary<RoleType, UnityEngine.Vector3>();
        private readonly Plugin plugin;
        public PlayerHandlers(Plugin plugin) => this.plugin = plugin;

        public void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;
            if (!this.plugin.zombies.ContainsKey(ev.Player.UserId))
            {
                this.plugin.zombies[ev.Player.UserId] = new Zombie(player.Id, player.Nickname, player.UserId, player.IPAddress);
            }
        }
        public void OnRoundEnd()
        {
            Spawns = new Dictionary<RoleType, UnityEngine.Vector3>();
            foreach (Player player in Exiled.API.Features.Player.List)
            {
                if (this.plugin.zombies.ContainsKey(player.UserId))
                {
                    this.plugin.zombies[player.UserId].Disconnected = false;
                }
            }
        }
        public void OnPlayerRoleChange(ChangingRoleEventArgs ev)
        {
            Player player = ev.Player;
            if  (this.plugin.zombies.ContainsKey(player.UserId))
            {
                if (plugin.zombies[player.UserId].Disconnected)
                {
                    plugin.zombies[player.UserId].Disconnected = false;
                    ev.NewRole = RoleType.Scp0492;
                }
            }
            
        }

        public void OnPlayerSpawn(SpawningEventArgs ev)
        {
            Player player = ev.Player;
            if (ev.RoleType == RoleType.Scp0492)
            {
                Player targetPlayer = GetTeleportTarget(player);
                if (targetPlayer != null)
                {
                    ev.Position = targetPlayer.Position;
                }
            }
            if (ev.RoleType == RoleType.Scp049)
            {
                player.Broadcast(10, "Use .recall to bring all your zombies to you");
            }
            if (ev.RoleType == RoleType.Scp173)
            {
                player.Broadcast(10, "Use .retreat to teleport to other SCPs");
            }

            if (Spawns.ContainsKey(ev.RoleType) == false)
            {
                Spawns.Add(ev.RoleType, ev.Position);
            };
        }

        public void OnPlayerDied(DiedEventArgs ev)
        {
            Player player = ev.Target;
            if (ev.Target.Role == RoleType.Scp0492)
            {
                    plugin.zombies[player.UserId].Disconnected = false;
            }
        }

        public void OnPlayerHurt(HurtingEventArgs ev)
        {
			if ((ev.DamageType == DamageTypes.Tesla || ev.DamageType == DamageTypes.Decont))
			{
				if (plugin.Config.HotlineCalls.ContainsKey(ev.Target.Role.ToString()) &&
					plugin.Config.HotlineCalls[ev.Target.Role.ToString()] != -1)
				{
					Player targetPlayer = GetTeleportTarget(ev.Target);
					if (targetPlayer != null)
					{
						ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
						ev.Target.Position = targetPlayer.Position;
					}
					else
					{
						// Do not warp SCP-173 back to Light Containment if decontaminated, but still apply the damage
						if (Map.IsLCZDecontaminated && ev.Target.Role == RoleType.Scp173)
						{
							ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
						}
						// Should be impossible to take tesla/wall/decont damage on nuked surface, but do not risk teleporting SCPs back
						else if (!Warhead.IsDetonated)
						{
							ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
							ev.Target.Position = Spawns[(ev.Target.Role)];
						}
					}
				}
			}
			else if (ev.DamageType == DamageTypes.Wall)
			{
				if (plugin.Config.HotlineCalls.ContainsKey(ev.Target.Role.ToString()) &&
					plugin.Config.HotlineCalls[ev.Target.Role.ToString()] != -1 &&
					ev.Amount > ev.Target.Health)
				{
					Player targetPlayer = GetTeleportTarget(ev.Target);
					if (targetPlayer != null)
					{
						ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
						ev.Target.Position = targetPlayer.Position;
					}
					else
					{
						// Do not warp SCP-173 back to Light Containment if decontaminated, but still apply the damage
						if (Map.IsLCZDecontaminated && ev.Target.Role == RoleType.Scp173)
						{
							ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
						}
						// Should be impossible to take tesla/wall/decont damage on nuked surface, but do not risk teleporting SCPs back
						else if (!Warhead.IsDetonated)
						{
							ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
							ev.Target.Position = Spawns[(ev.Target.Role)];
						}
					}
				}
			}
        }

        public void OnPlayerLeft(LeftEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp0492)
            {
                plugin.zombies[ev.Player.UserId].Disconnected = true;
            }
        }

        public Player GetTeleportTarget(Player sourcePlayer)
        {
            Player targetPlayer = null;
            foreach (Player player in Exiled.API.Features.Player.List)
            {
                if (sourcePlayer.UserId.Equals(player.UserId))
                {
                    continue;
                }

				// Do not teleport to computer
                if (player.Role == RoleType.Scp079)
                {
                    continue;
                }

                //if (targetPlayer == null)
                //{
                //    targetPlayer = player;
                //}

                if (player.Role == RoleType.Scp0492)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp096)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp106)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp173)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp93953)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp93989)
                {
                    targetPlayer = player;
                }

                if (player.Role == RoleType.Scp049)
                {
                    targetPlayer = player;
                    break;
                }
            }
            return targetPlayer;
        }
    }
}