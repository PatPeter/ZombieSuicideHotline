using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Events;
using Smod2.EventHandlers;
using System.Collections.Generic;
using ZombieSuicideHotline.EventHandlers;

namespace ZombieSuicideHotline
{
	[PluginDetails(
		author = "PatPeter",
		name = "ZombieSuicideHotline",
		description = "Respawns zombies that intentionally kill themselves.",
		id = "patpeter.zombie.suicide.hotline",
		version = "1.6.0.44",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 0
		)]
	class ZombieSuicideHotlinePlugin : Plugin
    {
		internal bool duringRound = false;
		//internal HashSet<string> scp049Kills = new HashSet<string>();
		//internal HashSet<string> zombieDisconnects = new HashSet<string>();
		internal int lastRecall = 0;
		internal bool ProcessingDisconnect = false;

		internal Dictionary<Role, Vector> scpSpawns = new Dictionary<Role, Vector>();

		internal Dictionary<string, Zombie> zombies = new Dictionary<string, Zombie>();

		public override void OnEnable()
        {
            this.Info("Zombie Suicide Hotline has loaded :)");
            this.Info("Config vale: " + this.GetConfigBool("zombie_suicide_hotline_enabled"));
        }

        public override void OnDisable()
		{

		}

		public override void Register()
		{
            // Register Events
            this.AddEventHandler(typeof(IEventHandlerRoundStart), new RoundStartHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerRoundEnd), new RoundEndHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerPlayerJoin), new PlayerJoinHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerDisconnect), new DisconnectHandler(this), Priority.Highest);
			this.AddEventHandler(typeof(IEventHandlerSpawn), new SpawnHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerSetRole), new SetRoleHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerTeamRespawn), new TeamRespawnHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerPlayerDie), new PlayerDieHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerPlayerHurt), new PlayerHurtHandler(this), Priority.Lowest);
			this.AddEventHandler(typeof(IEventHandlerCallCommand), new CallCommandHandler(this), Priority.Lowest);

			// Register config settings
			this.AddConfig(new Smod2.Config.ConfigSetting("zombie_suicide_hotline_enabled", true, Smod2.Config.SettingType.BOOL, true, "Enables or disables the zombie suicide hotline."));

			this.AddConfig(new Smod2.Config.ConfigSetting("zombie_suicide_hotline_recall", true, Smod2.Config.SettingType.NUMERIC, true, "Time delay on recall commands for SCP-049."));
		}

		public Player getTeleportTarget(Player sourcePlayer)
		{
			Player targetPlayer = null;
			//TeamRole lastTeamRole = null;
			foreach (Player player in this.Server.GetPlayers())
			{
				if (sourcePlayer.SteamId.Equals(player.SteamId))
				{
					continue;
				}

				if (player.TeamRole.Role == Role.SCP_079)
				{
					continue;
				}

				if (targetPlayer == null)
				{
					targetPlayer = player;
				}

				if (player.TeamRole.Team == Team.SCP)
				{
					targetPlayer = player;
				}

				if (player.TeamRole.Role == Role.SCP_049)
				{
					targetPlayer = player;
					break;
				}
			}
			return targetPlayer;
		}
	}

	class Zombie
	{
		public int PlayerId;
		public string Name;
		public string SteamId;
		public string IpAddress;
		public int LastRecall = 0;
		public bool Undead = false;
		public bool Disconnected = false;
		public Dictionary<string, Zombie> Zombies = new Dictionary<string, Zombie>();

		public Zombie(int playerId, string name, string steamId, string ipAddress)
		{
			this.PlayerId = playerId;
			this.Name = name;
			this.SteamId = steamId;
			this.IpAddress = ipAddress;
		}

		
		public override string ToString()
		{
			return "[ PlayerId: " + PlayerId + ", Name: " + Name + ", SteamID: " + SteamId + ", IpAddress: " + IpAddress + ", LastRecall: " + LastRecall + ", Undead: " + Undead + ", Disconnected: " + Disconnected + ", Zombies Size: " + Zombies.Count + " ]";
		}
	}
}
