using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RemoteAdmin;
using UnityEngine;

namespace ZombieSuicideHotline
{
	[CommandHandler(typeof(ClientCommandHandler))]
	class VentCommand : ICommand
	{
		public string Command => "vent";

		public string[] Aliases => new string[] { "retreat", "amogus" };

		public string Description => "Allows SCP-173 to teleport to other SCPs.";

		public float LastTime = 0;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "";
			if (Plugin.Instance.Config.AllowVent)
			{
				Player player = Player.Get(((CommandSender)sender).SenderId);
				if (player.Role == RoleTypeId.Scp173)
				{
					Player ScpTpPlayer = GetTeleportTarget(player);
					if (ScpTpPlayer != null)
					{
						if (TimerFunction())
						{
							player.Position = ScpTpPlayer.Position; //ScpTpPlayer.ReferenceHub.playerMovementSync.LastGroundedPosition;
							response = "Escaped!";
						}
						else
						{
							response = "vent is on cooldown for " + (LastTime + Plugin.Instance.Config.VentCooldown - Time.time).ToString();
						}
					}
					if (response == "")
					{
						response = "No alive SCPs!";
					}
				}
				else
				{
					response = "You must be SCP 173 to use this command!";
				}
			}
			else
			{
				response = ".vent is not enabled.";
			}
			return true;
		}

		public bool TimerFunction()
		{
			if (LastTime + Plugin.Instance.Config.VentCooldown < Time.time)
			{
				LastTime = Time.time;
				return true;
			}
			return false;
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

				if (player.Role == RoleTypeId.Scp079)
				{
					continue;
				}

				if (player.Role.Is(out Exiled.API.Features.Roles.Scp106Role scp106Role))
				{
					if (scp106Role.IsSubmerged)
					{
						continue;
					}
				}

				if (player.Role.Team == Team.SCPs)
				{
					targetPlayer = player;
					break;
				}
			}
			return targetPlayer;
		}
	}
}
