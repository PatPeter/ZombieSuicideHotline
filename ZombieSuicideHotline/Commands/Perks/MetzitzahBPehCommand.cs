using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZombieSuicideHotline.Commands.Perks
{
	[CommandHandler(typeof(ClientCommandHandler))]
	class MetzitzahBPehCommand : ICommand
	{
		public string Command => "metzitzahbpeh";

		public string[] Aliases => new string[] { "mbp" };

		public string Description => "Circumcise your zombies and suck out their blood to gain health.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "";
			if (Plugin.Instance.Config.AllowMetzitzahBPeh)
			{
				Player player = Player.Get(((CommandSender)sender).SenderId);
				if (player.Role.Type == RoleTypeId.Scp049)
				{
					if (TimerFunction())
					{
						float totalHealthGained = 0f;
						foreach (Player ply in Exiled.API.Features.Player.List)
						{
							if (ply.Role.Type == RoleTypeId.Scp0492 && Plugin.Instance.PlayerHandlers.DoctorsZombies[player.UserId].Contains(ply.UserId))
							{
								float tempHealth = ply.Health * ((float) Plugin.Instance.Config.MetzitzahBPehPercentage / 100);
								ply.Hurt(tempHealth);
								player.Heal(tempHealth, true);
								totalHealthGained += tempHealth;
								response = $"You've become a mohel and gained {totalHealthGained} from circumcising your zombies and sucking their penis blood!";
							}
						}
						Log.Info($"{player.Nickname} ran .{Command} and gained {totalHealthGained} HP.");
					}
					else
					{
						response = $"{Command} is on cooldown for " + (LastTime + Plugin.Instance.Config.CommandCooldowns[Command] - Time.time).ToString();
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
				response = $".{Command} is not enabled.";
			}
			return true;
		}

		public float LastTime = 0;

		public bool TimerFunction()
		{
			if (LastTime + Plugin.Instance.Config.CommandCooldowns[Command] < Time.time)
			{
				LastTime = Time.time;
				return true;
			}
			return false;
		}
	}
}
