using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Text.RegularExpressions;
using System.Linq;
using Smod2.EventSystem.Events;
using System.Collections.Generic;

namespace ZombieSuicideHotline.EventHandlers
{
	class RoundStartHandler : IEventHandlerRoundStart
	{
		private ZombieSuicideHotlinePlugin plugin;

		public RoundStartHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin) plugin;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			this.plugin.duringRound = true;
			this.plugin.zombies = new Dictionary<string, Zombie>();
			//this.plugin.scp049Kills = new System.Collections.Generic.HashSet<string>();
			//this.plugin.zombieDisconnects = new System.Collections.Generic.HashSet<string>();
			this.plugin.lastRecall = 0;
		}
	}

	class RoundEndHandler : IEventHandlerRoundEnd
	{
		private ZombieSuicideHotlinePlugin plugin;

		public RoundEndHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin) plugin;
		}

		public void OnRoundEnd(RoundEndEvent ev)
		{

			if (ev.Round.Duration >= 3)
			{
				this.plugin.duringRound = false;
				this.plugin.zombies = new Dictionary<string, Zombie>();
			}
		}
	}

	/**
	 * 3
	 */
	class PlayerJoinHandler : IEventHandlerPlayerJoin
	{
		private ZombieSuicideHotlinePlugin plugin;

		public PlayerJoinHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			Player player = ev.Player;
			if (!this.plugin.zombies.ContainsKey(ev.Player.SteamId))
			{
				this.plugin.zombies[ev.Player.SteamId] = new Zombie(player.PlayerId, player.Name, player.SteamId, player.IpAddress);
			}
			/*else
			{
				if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled") && this.plugin.duringRound && this.plugin.zombies[ev.Player.SteamId].Undead)
				{
					//this.plugin.Info("[OnPlayerJoin] Removing player [" + ev.Player.IpAddress + "] from zombieDisconnects.");
					//this.plugin.zombieDisconnects.Remove(ev.Player.IpAddress);
					int counter = 0;
					do
					{
						this.plugin.Info("[OnPlayerJoin] Making " + counter + " attempt(s) to respawn " + ev.Player.Name + " [" + ev.Player.SteamId + "] as a zombie.");
						ev.Player.ChangeRole(Role.SCP_049_2, true, true);
						counter++;
					} while (ev.Player.TeamRole.Role != Role.SCP_049_2 && counter < 5);
				}
			}*/
		}
	}

	/*
	 * ALGORITHM:
	 * 1.  If player dies, add to list of scp049Kills.
	 * 2a. If a player dies as SCP-049-2, remove from scp049Kills
	 * 2b. If player killed by SCP-049 disconnects, remove from scp049Kills
	 *      and add to zombieDisconnects.
	 * 3.  If the player reconnects to the server, attempt to respawn the
	 *     player as SCP-049-2.
	 */

	/**
	 * 1
	 */
	class PlayerDieHandler : IEventHandlerPlayerDie
	{
		private ZombieSuicideHotlinePlugin plugin;

		public PlayerDieHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled") && this.plugin.duringRound)
			{
				Player player = ev.Player;
				if (ev.Killer.TeamRole.Role == Role.SCP_049)
				{
					this.plugin.Info("[OnPlayerDie] Adding player " + player.Name + " [" + ev.Player.SteamId + "] to scp049Kills.");
					//this.plugin.scp049Kills.Add(ev.Player.IpAddress);
					if (this.plugin.zombies.ContainsKey(player.SteamId))
					{
						this.plugin.zombies[player.SteamId].Undead = true;
					}
					else
					{
						this.plugin.zombies[player.SteamId] = new Zombie(player.PlayerId, player.Name, player.SteamId, player.IpAddress);
						this.plugin.zombies[player.SteamId].Undead = true;
					}
				}
				else if (ev.Player.TeamRole.Role == Role.SCP_049_2)
				{
					this.plugin.Info("[OnSetClass] Removing player " + player.Name + "[" + ev.Player.SteamId + "] from zombieDisconnects for dying as a zombie.");
					//this.plugin.zombieDisconnects.Remove(ev.Player.IpAddress);
					if (this.plugin.zombies.ContainsKey(player.SteamId))
					{
						this.plugin.zombies[player.SteamId].Undead = false;
						this.plugin.zombies[player.SteamId].Disconnected = false;
					}
					else
					{
						this.plugin.zombies[player.SteamId] = new Zombie(player.PlayerId, player.Name, player.SteamId, player.IpAddress);
						this.plugin.zombies[player.SteamId].Undead = false;
						this.plugin.zombies[player.SteamId].Disconnected = false;
					}
				}
			}
		}
	}

	/**
	 * 2a
	 */
	class SetRoleHandler : IEventHandlerSetRole
	{
		private ZombieSuicideHotlinePlugin plugin;

		public SetRoleHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			Player player = ev.Player;
			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled"))
			{
				if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Undead && ev.Role != Role.SPECTATOR)
				{
					this.plugin.Info("[OnSetRole] Player " + player.Name + " [" + player.SteamId + "] spawned naturally, unflag as undead...");
					this.plugin.zombies[player.SteamId].Undead = false;
				}
				else if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Disconnected && ev.Role != Role.SCP_049_2)
				{
					//this.plugin.Info("[OnSetClass] Removing player [" + ev.Player.IpAddress + "] from zombieDisconnects.");
					//this.plugin.zombieDisconnects.Remove(ev.Player.IpAddress);
					this.plugin.Info("[OnSetRole] Force spawning disconnecting player " + player.Name + " [" + player.SteamId + "] as zombie...");
					ev.Role = Role.SCP_049_2;
					//ev.Player.ChangeRole(Role.SCP_049_2);
					Player targetPlayer = this.plugin.getTeleportTarget(player);
					player.Teleport(targetPlayer.GetPosition());
				}
			}
		}
	}

	/**
	 * 2a
	 */
	class SpawnHandler : IEventHandlerSpawn
	{
		private ZombieSuicideHotlinePlugin plugin;

		public SpawnHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnSpawn(PlayerSpawnEvent ev)
		{
			Player player = ev.Player;

			if (ev.Player.TeamRole.Team == Team.SCP)
			{
				this.plugin.scpSpawns[ev.Player.TeamRole.Role] = ev.Player.GetPosition();
			}

			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled"))
			{
				if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Undead && player.TeamRole.Role != Role.SPECTATOR)
				{
					this.plugin.Info("[OnSpawn] Player " + player.Name + " [" + player.SteamId + "] spawned naturally, unflag as undead...");
					this.plugin.zombies[player.SteamId].Undead = false;
				}
				else if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Disconnected && player.TeamRole.Role != Role.SCP_049_2)
				{
					//plugin.Info("[OnPlayerLeave] Removing player [" + ev.Player.IpAddress + "] from scp049Kills for spawning in as " + Role.SCP_049_2 + ".");
					//this.plugin.scp049Kills.Remove(ev.Player.IpAddress);
					this.plugin.Info("[OnSpawn] Force spawning disconnecting player " + player.Name + " [" + player.SteamId + "] as zombie...");
					ev.Player.ChangeRole(Role.SCP_049_2);
					ev.Player.SetHealth(this.plugin.GetConfigInt("scp049-2_hp"));
					//Player targetPlayer = this.plugin.getTeleportTarget(player);
					//player.Teleport(targetPlayer.GetPosition());
				}
			}
		}
	}

	/**
	 * 2a
	 */
	class TeamRespawnHandler : IEventHandlerTeamRespawn
	{
		private ZombieSuicideHotlinePlugin plugin;

		public TeamRespawnHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled"))
			{
				foreach (Player player in ev.PlayerList)
				{
					if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Undead && player.TeamRole.Role != Role.SPECTATOR)
					{
						this.plugin.Info("[OnTeamRespawn] Player " + player.Name + " [" + player.SteamId + "] spawned naturally, unflag as undead...");
						this.plugin.zombies[player.SteamId].Undead = false;
					}
					else if (this.plugin.duringRound && this.plugin.zombies.ContainsKey(player.SteamId) && this.plugin.zombies[player.SteamId].Disconnected && player.TeamRole.Role != Role.SCP_049_2)
					{
						//this.plugin.Info("[OnPlayerLeave] Removing player [" + player.IpAddress + "] from scp049Kills for spawning in as " + player.TeamRole.Role + ".");
						//this.plugin.scp049Kills.Remove(player.IpAddress);
						this.plugin.Info("[OnTeamRespawn] Force spawning disconnecting player " + player.Name + " [" + player.SteamId + "] as zombie...");
						player.ChangeRole(Role.SCP_049_2);
						Player targetPlayer = this.plugin.getTeleportTarget(player);
						player.Teleport(targetPlayer.GetPosition());
					}
				}
			}
		}
	}

	/**
	 * 2b
	 */
	class DisconnectHandler : IEventHandlerDisconnect
	{
		private ZombieSuicideHotlinePlugin plugin;

		public DisconnectHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnDisconnect(DisconnectEvent ev)
		{
			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled") && this.plugin.duringRound && !this.plugin.ProcessingDisconnect)
			{
				this.plugin.ProcessingDisconnect = true;

				if (ev.Connection.IpAddress != null && !"".Equals(ev.Connection.IpAddress))
				{
					// Users on LAN can have the same IP address
					List<Zombie> disconnectedIpAddresses = this.plugin.zombies.Values.Where(zombie => zombie.IpAddress.Equals(ev.Connection.IpAddress)).ToList();
					//if (disconnectedIpAddresses.Count > 0)
					//{
					//	this.plugin.Info("[OnDisconnect] Found a total of " + disconnectedIpAddresses.Count + " disconnected IP addresses.");
					//}

					foreach (Zombie z in disconnectedIpAddresses)
					{
						if (z.Undead)
						{
							z.Undead = false;
							z.Disconnected = true;
							this.plugin.Info("[OnDisconnect] Found disconnected IP address belonging to " + z.Name + " [" + z.IpAddress + "] and set disconnect flag.");
						}
					}
				}
				else
				{

					/*this.plugin.Info("[OnDisconnect] Player List: ");
					for (int i = 0; i < this.plugin.Server.GetPlayers().Count; i++)
					{
						this.plugin.Info("[OnDisconnect] Player " + i + ": " + this.plugin.Server.GetPlayers()[i]);
					}

					this.plugin.Info("[OnDisconnect] Zombie List: ");
					int counter = 0;
					foreach (Zombie zombie in this.plugin.zombies.Values)
					{
						this.plugin.Info("[OnDisconnect] Zombie " + counter + ": " + zombie);
						counter++;
					}*/

					List<Zombie> disconnectedUsers = this.plugin.zombies.Values.Where(zombie => !this.plugin.Server.GetPlayers().Any(p => zombie.SteamId == p.SteamId)).ToList();
					//if (disconnectedUsers.Count > 0)
					//{
					//	this.plugin.Info("[OnDisconnect] Found a total of " + disconnectedUsers.Count + " disconnected users.");
					//}

					foreach (Zombie z in disconnectedUsers)
					{
						if (z.Undead)
						{
							//this.plugin.Info("[OnDisconnect] Removing player " + z.Name + " [" + z.SteamId + "] from scp049Kills after disconnecting.");
							z.Undead = false;
							//this.plugin.Info("[OnDisconnect] Adding player " + z.Name + " [" + z.SteamId + "] to zombieDisconnects for leaving.");
							z.Disconnected = true;
							this.plugin.Info("[OnDisconnect] Found disconnected user " + z.Name + " [" + z.SteamId + "] and set disconnect flag.");
						}
						//else
						//{
						//	this.plugin.Info("[OnDisconnect] Player " + z.Name + " [" + z.SteamId + "] was not killed by SCP-049. Do nothing.");
						//}
						//plugin.Info("[OnPlayerLeave] Removing player [" + ev.Connection.IpAddress + "] from scp049Kills after disconnecting.");
						//this.plugin.scp049Kills.Remove(ev.Connection.IpAddress);
						//plugin.Info("[OnPlayerLeave] Adding player [" + ev.Connection.IpAddress + "] to zombieDisconnects for leaving.");
						//this.plugin.zombieDisconnects.Add(ev.Connection.IpAddress);
					}
				}

				this.plugin.ProcessingDisconnect = false;
			}
		}
	}

	class PlayerHurtHandler : IEventHandlerPlayerHurt
	{
		private ZombieSuicideHotlinePlugin plugin;

		public PlayerHurtHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin) plugin;
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled"))
			{
				if (ev.Player.TeamRole.Role == Role.SCP_049_2 && (ev.DamageType == DamageType.TESLA || ev.DamageType == DamageType.WALL))
				{
					Player targetPlayer = this.plugin.getTeleportTarget(ev.Player);

					if (targetPlayer != null)
					{
						ev.Damage = 0;
						ev.Player.Teleport(targetPlayer.GetPosition());
					}
				}
			}
		}
	}

	class CallCommandHandler : IEventHandlerCallCommand
	{
		private ZombieSuicideHotlinePlugin plugin;

		public CallCommandHandler(Plugin plugin)
		{
			this.plugin = (ZombieSuicideHotlinePlugin)plugin;
		}

		public void OnCallCommand(PlayerCallCommandEvent ev)
		{
			string command = ev.Command.Split(' ')[0];
			string[] quotedArgs = Regex.Matches(ev.Command, "[^\\s\"\']+|\"([^\"]*)\"|\'([^\']*)\'")
				.Cast<Match>()
				.Select(m => m.Value)
				.ToArray()
				.Skip(1)
				.ToArray();

			switch (command)
			{
				case "recall":
					if (this.plugin.GetConfigBool("zombie_suicide_hotline_enabled"))
					{
						if (ev.Player.TeamRole.Role == Role.SCP_049)
						{
							if (this.plugin.lastRecall == 0 || this.plugin.lastRecall + 30 <= this.plugin.Server.Round.Duration)
							{
								bool zombiesFound = false;
								foreach (Player checkZombie in this.plugin.Server.GetPlayers())
								{
									if (checkZombie.TeamRole.Role == Role.SCP_049_2)
									{
										checkZombie.Teleport(ev.Player.GetPosition(), true);
										zombiesFound = true;
									}
								}

								if (zombiesFound)
								{
									ev.ReturnMessage = "All of your SCP-049-2 have been teleported to you.";
									this.plugin.lastRecall = this.plugin.Server.Round.Duration;
								}
								else
								{
									ev.ReturnMessage = "You have no SCP-049-2 alive right now.";
								}
							}
							else
							{
								ev.ReturnMessage = "You must wait " + ((this.plugin.lastRecall + 15) - this.plugin.Server.Round.Duration) + " seconds before using .recall again.";
							}
						}
						else
						{
							ev.ReturnMessage = "You cannot recall zombies unless you are SCP-049.";
						}
					}
					else
					{
						ev.ReturnMessage = "Zombie Suicide Hotline is currently disabled.";
					}
					break;
			}
		}
	}
}
