﻿using System;
using System.Collections.Generic;

namespace ZombieSuicideHotline
{
	using Exiled.API.Features;
	using UnityEngine;
	using Player = Exiled.Events.Handlers.Player;
	using Server = Exiled.Events.Handlers.Server;

	public class Plugin : Plugin<Config>
    {
        public override string Name { get; } = ZombieSuicideHotline.AssemblyInfo.Name;
        public override string Author { get; } = ZombieSuicideHotline.AssemblyInfo.Author;
        public override Version Version { get; } = new Version(ZombieSuicideHotline.AssemblyInfo.Version);
        //public override Version RequiredExiledVersion { get; } = new Version(2, 0, 12);
        public override string Prefix { get; } = ZombieSuicideHotline.AssemblyInfo.ConfigPrefix;

        public PlayerHandlers PlayerHandlers;
        public Dictionary<string, Zombie> zombies = new Dictionary<string, Zombie>();
        public static Plugin Singleton;

		internal Dictionary<RoleType, Vector3> scpSpawns = new Dictionary<RoleType, Vector3>();

		public override void OnEnabled()
        {
            Singleton = this;

            Log.Info($"Instantiating Events..");
            PlayerHandlers = new PlayerHandlers(this);

            Log.Info($"Registering EventHandlers..");
            Player.Verified += PlayerHandlers.OnPlayerVerified;
            Player.Died += PlayerHandlers.OnPlayerDied;
            Player.Left += PlayerHandlers.OnPlayerLeft;
            Player.Hurting += PlayerHandlers.OnPlayerHurt;
            Player.ChangingRole += PlayerHandlers.OnPlayerRoleChange;
            Player.Spawning += PlayerHandlers.OnPlayerSpawn;
            Server.RoundStarted += PlayerHandlers.OnRoundEnd;
        }
        public override void OnDisabled()
        {
            Player.Verified -= PlayerHandlers.OnPlayerVerified;
            Player.Died -= PlayerHandlers.OnPlayerDied;
            Player.Left -= PlayerHandlers.OnPlayerLeft;
            Player.Hurting -= PlayerHandlers.OnPlayerHurt;
            Player.ChangingRole -= PlayerHandlers.OnPlayerRoleChange;
            Player.Spawning -= PlayerHandlers.OnPlayerSpawn;
            Server.RoundStarted -= PlayerHandlers.OnRoundEnd;

        }
    }

    public class Zombie
    {
        public int PlayerId;
        public string Name;
        public string SteamId;
        public string IpAddress;
        public int LastRecall = 0;
        public bool Undead = false;
        public bool Disconnected = false;
        //public Dictionary<string, Zombie> Zombies = new Dictionary<string, Zombie>();

        public Zombie(int playerId, string name, string steamId, string ipAddress)
        {
            this.PlayerId = playerId;
            this.Name = name;
            this.SteamId = steamId;
            this.IpAddress = ipAddress;
        }


        public override string ToString()
        {
            return "[ PlayerId: " + PlayerId + ", Name: " + Name + ", SteamID: " + SteamId + ", IpAddress: " + IpAddress + ", LastRecall: " + LastRecall + ", Undead: " + Undead + ", Disconnected: " + Disconnected + " ]";
        }
    }
}
