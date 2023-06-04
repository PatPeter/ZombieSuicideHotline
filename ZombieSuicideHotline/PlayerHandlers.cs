namespace ZombieSuicideHotline
{
	using Exiled.Events.EventArgs;
	using Player = Exiled.API.Features.Player;
	using Exiled.API.Features;
	using System.Collections.Generic;
	using System;
	using System.Linq;
	using Exiled.Events.EventArgs.Player;
	using PlayerRoles;
	using Exiled.Events.EventArgs.Scp049;
	using Exiled.API.Features.DamageHandlers;

	public class PlayerHandlers
	{
		IDictionary<RoleTypeId, UnityEngine.Vector3> Spawns = new Dictionary<RoleTypeId, UnityEngine.Vector3>();
		private readonly Plugin plugin;
		public Dictionary<string, List<string>> DoctorsZombies = new Dictionary<string, List<string>>();
		public PlayerHandlers(Plugin plugin) => this.plugin = plugin;

		public void OnPlayerVerified(VerifiedEventArgs ev)
		{
			Player player = ev.Player;
			if (!this.plugin.Zombies.ContainsKey(ev.Player.UserId))
			{
				this.plugin.Zombies[ev.Player.UserId] = new Zombie(player.Id, player.Nickname, player.UserId, player.IPAddress);
			}
		}

		public void OnRoundEnd()
		{
			Spawns = new Dictionary<RoleTypeId, UnityEngine.Vector3>();
			foreach (Player player in Exiled.API.Features.Player.List)
			{
				if (this.plugin.Zombies.ContainsKey(player.UserId) && this.plugin.Config.RespawnZombieRagequits)
				{
					this.plugin.Zombies[player.UserId].Disconnected = false;
				}
			}
			DoctorsZombies = new Dictionary<string, List<string>>();
		}

		public void OnPlayerRoleChange(ChangingRoleEventArgs ev)
		{
			Player player = ev.Player;
			if  (this.plugin.Zombies.ContainsKey(player.UserId))
			{
				if (plugin.Zombies[player.UserId].Disconnected && this.plugin.Config.RespawnZombieRagequits)
				{
					plugin.Zombies[player.UserId].Disconnected = false;
					ev.NewRole = RoleTypeId.Scp0492;
				}
			}
		}

		public void OnPlayerSpawn(SpawningEventArgs ev)
		{
			Player player = ev.Player;
			RoleTypeId role = ev.Player.Role.Type;
			if (ev.Player.Role.Type == RoleTypeId.Scp0492)
			{
				Player targetPlayer = GetTeleportTarget(player);
				if (targetPlayer != null)
				{
					ev.Position = targetPlayer.Position;
				}
			}
			if (role == RoleTypeId.Scp049)
			{
				player.Broadcast(10, $"<size=16>Press ~ and type .recall to bring all your zombies to you. Type .passover to kill your firstborn SCP-049-2 and absorb his health. Type .mbp to circumcise your SCP-049-2 and suck their blood for {Plugin.Instance.Config.MetzitzahBPehPercentage}% health each.</size>");
			}
			if (ev.Player.Role.Type == RoleTypeId.Scp173)
			{
				player.Broadcast(10, "Press ~ and type .vent to teleport to other SCPs.");
			}

			if (Spawns.ContainsKey(ev.Player.Role.Type) == false)
			{
				Spawns.Add(ev.Player.Role.Type, ev.Position);
			};
		}

		public void OnDoctorRevive(FinishingRecallEventArgs ev)
		{
			if(DoctorsZombies.ContainsKey(ev.Player.UserId))
			{
				DoctorsZombies[ev.Player.UserId].Add(ev.Target.UserId);
			}
			else
			{
				DoctorsZombies[ev.Player.UserId] = new List<string>{ ev.Target.UserId };
			}

			foreach (Player zombie in Exiled.API.Features.Player.List)
			{
				if (zombie.Role.Type == RoleTypeId.Scp0492 && DoctorsZombies[ev.Player.UserId].Any(uid => uid == zombie.UserId))
				{
					int healthBonus = Plugin.Instance.Config.BonusReviveHealth + (Plugin.Instance.Config.PerZombieBonusHealth * DoctorsZombies[ev.Player.UserId].Count);
					zombie.Heal(healthBonus, true);
					zombie.Broadcast(new Broadcast($"You've been given {healthBonus} health because the doctor now has {DoctorsZombies[ev.Player.UserId].Count} zombies alive.", 3));
				}
			}
		}

		public void OnPlayerHurt(HurtingEventArgs ev)
		{
			Player target = ev.Player;
			if (target == null)
			{
				Log.Error("[OnPlayerHurt] Target of hurt was null in zombie event.");
				return;
			}
			
			if (ev.DamageHandler.Type == Exiled.API.Enums.DamageType.Tesla || 
				ev.DamageHandler.Type == Exiled.API.Enums.DamageType.Crushed || 
				ev.DamageHandler.Type == Exiled.API.Enums.DamageType.Decontamination)
			{
				Log.Debug($"Checking damage type {ev.DamageHandler.Type} damage {ev.DamageHandler.Damage}...");
				if (plugin.Config.HotlineCalls.ContainsKey(target.Role.Type.ToString()) && plugin.Config.HotlineCalls[target.Role.Type.ToString()] != -1) 
				{
					if (Warhead.IsDetonated != true && (Map.IsLczDecontaminated != true || target.Role.Type != RoleTypeId.Scp173) && target.Role.Type != RoleTypeId.Scp0492)
					{
						ev.Amount = (target.Health * plugin.Config.HotlineCalls[target.Role.Type.ToString()]);
						target.Position = Spawns[target.Role.Type];
					}
					else
					{
						Player targetPlayer = GetTeleportTarget(target);
						if (targetPlayer != null)
						{
							ev.Amount = (target.Health * plugin.Config.HotlineCalls[target.Role.Type.ToString()]);
							target.Position = targetPlayer.Position; // targetPlayer.ReferenceHub.playerMovementSync.LastGroundedPosition;
						}
					}
				} 
			}
		}

		public void OnPlayerDying(DyingEventArgs ev)
		{
			Player target = ev.Player;
			if (target == null)
			{
				Log.Error("[OnPlayerDying] Target of death was null in zombie event.");
				return;
			}
			Player attacker = ev.Attacker;
			CustomDamageHandler dh = ev.DamageHandler;

			Log.Info($"Player {target.Nickname} playing {target.Role.Type} died to {dh.Type} after taking {dh.Damage} damage.");
			if (target.Role.Type == RoleTypeId.Scp0492)
			{
				if (this.plugin.Zombies.ContainsKey(target.UserId) && this.plugin.Config.RespawnZombieRagequits)
				{
					plugin.Zombies[target.UserId].Disconnected = false;
				}

				if (DoctorsZombies.Keys.Count > 0) {
					Player scp049 = Exiled.API.Features.Player.List.Where(z => z.UserId == DoctorsZombies.Keys.First()).First();
					if (scp049 != null && attacker != null)
					{
						scp049.Broadcast(new Broadcast($"Your son {target.Nickname} has been circumcised by the {attacker.Role.Team}!", 3));
					}
				}
			}
		}

		public void OnPlayerLeft(LeftEventArgs ev)
		{
			if (ev.Player.Role.Type == RoleTypeId.Scp0492)
			{
				if (this.plugin.Zombies.ContainsKey(ev.Player.UserId) && this.plugin.Config.RespawnZombieRagequits)
				{
					plugin.Zombies[ev.Player.UserId].Disconnected = true;
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

				// Do not teleport to computer
				if (player.Role.Type == RoleTypeId.Scp079)
				{
					continue;
				}

				//if (targetPlayer == null)
				//{
				//	targetPlayer = player;
				//}

				if (player.Role.Type == RoleTypeId.Scp0492)
				{
					targetPlayer = player;
				}

				if (player.Role.Type == RoleTypeId.Scp096)
				{
					targetPlayer = player;
				}

				if (player.Role.Type == RoleTypeId.Scp106)
				{
					targetPlayer = player;
				}

				if (player.Role.Type == RoleTypeId.Scp173)
				{
					targetPlayer = player;
				}

				if (player.Role.Type == RoleTypeId.Scp939)
				{
					targetPlayer = player;
				}

				if (player.Role.Type == RoleTypeId.Scp049)
				{
					targetPlayer = player;
					break;
				}
			}
			return targetPlayer;
		}
	}
}
