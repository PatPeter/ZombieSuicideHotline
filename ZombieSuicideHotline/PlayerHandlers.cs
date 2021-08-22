namespace ZombieSuicideHotline
{
    using Exiled.Events.EventArgs;
    using Player = Exiled.API.Features.Player;
    using Exiled.API.Features;
    using System.Collections.Generic;

    public class PlayerHandlers
    {
        IDictionary<RoleType, UnityEngine.Vector3> Spawns = new Dictionary<RoleType, UnityEngine.Vector3>();
        private readonly Plugin plugin;
        public Dictionary<string, List<string>> DoctorsZombies = new Dictionary<string, List<string>>();
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
            DoctorsZombies = new Dictionary<string, List<string>>();
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
                player.Broadcast(10, "Use .vent to teleport to other SCPs");
            }

            if (Spawns.ContainsKey(ev.RoleType) == false)
            {
                Spawns.Add(ev.RoleType, ev.Position);
                Log.Error(ev.RoleType);
            };
        }

        public void OnPlayerDied(DiedEventArgs ev)
        {
            Player player = ev.Target;
            if (ev.Target.Role == RoleType.Scp0492)
            {
                if (this.plugin.zombies.ContainsKey(player.UserId))
                {
                    plugin.zombies[player.UserId].Disconnected = false;
                }
            }
        }

        public void OnDoctorRevive(FinishingRecallEventArgs ev)
        {
            if(DoctorsZombies.ContainsKey(ev.Scp049.UserId))
            {
                DoctorsZombies[ev.Scp049.UserId].Add(ev.Target.UserId);
            }
            else
            {
                DoctorsZombies[ev.Scp049.UserId] = new List<string>{ ev.Target.UserId };
            }
        }

        public void OnPlayerHurt(HurtingEventArgs ev)
        {
            if ((ev.DamageType == DamageTypes.Tesla || (ev.DamageType == DamageTypes.Wall && ev.Amount > 10000) || ev.DamageType == DamageTypes.Decont))
                {
                if (plugin.Config.HotlineCalls.ContainsKey(ev.Target.Role.ToString()) && plugin.Config.HotlineCalls[ev.Target.Role.ToString()] != -1) 
                {
                    
                    if (Warhead.IsDetonated != true && (Map.IsLczDecontaminated != true || ev.Target.Role != RoleType.Scp173) && ev.Target.Role != RoleType.Scp0492)
                    {
                        ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
                        Log.Error("ran");
                        Log.Error(ev.Target.Role);
                        ev.Target.Position = Spawns[ev.Target.Role];
                        Log.Error("after");
                    }
                    else
                    {
                        Player targetPlayer = GetTeleportTarget(ev.Target);
                        if (targetPlayer != null)
                        {
                            ev.Amount = (ev.Target.Health * plugin.Config.HotlineCalls[ev.Target.Role.ToString()]);
                            ev.Target.Position = targetPlayer.Position;
                        }
                    }
                } 
            }
        }

        public void OnPlayerLeft(LeftEventArgs ev)
        {
            if (ev.Player.Role == RoleType.Scp0492)
            {
                if (this.plugin.zombies.ContainsKey(ev.Player.UserId))
                {
                    plugin.zombies[ev.Player.UserId].Disconnected = true;
                }
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

                if (player.Role == RoleType.Scp079)
                {
                    continue;
                }

                if (targetPlayer == null)
                {
                    targetPlayer = player;
                }

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