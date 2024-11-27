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
	class RecallCommand : ICommand
	{
		public bool SanitizeResponse => false;
		public string Command => "recall";

		public string[] Aliases => new string[] { "rc" };

		public string Description => "Allows you to bring all zombies to you as SCP-049.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "";
			if (Plugin.Instance.Config.AllowVent)
			{
				Player player = Player.Get(((CommandSender)sender).SenderId);
				if (player.Role.Type == RoleTypeId.Scp049)
				{
					if (TimerFunction())
					{
						foreach (Player players in Exiled.API.Features.Player.List)
						{
							if (players.Role.Type == RoleTypeId.Scp0492 && Plugin.Instance.PlayerHandlers.DoctorsZombies[player.UserId].Contains(players.UserId))
							{
								players.Position = player.Position;
								response = "Zombies recalled!";
							}
						}
					}
					else
					{
						response = "Recall is on cooldown for " + (LastTime + Plugin.Instance.Config.RecallCooldown - Time.time).ToString();
					}
					if (response == "")
					{
						response = "No alive Zombies!";
					}
				}
				else
				{
					response = "You must be SCP-049 to use this command!";
				}
			}
			else
			{
				response = ".recall is not enabled.";
			}
			return true;
		}

		public float LastTime = 0;

		public bool TimerFunction()
		{
			if (LastTime + Plugin.Instance.Config.RecallCooldown < Time.time)
			{
				LastTime = Time.time;
				return true;
			}
			return false;
		}
	}
}
