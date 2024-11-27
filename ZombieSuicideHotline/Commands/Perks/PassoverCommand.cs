using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PlayerRoles;

namespace ZombieSuicideHotline.Commands.Perks
{
	[CommandHandler(typeof(ClientCommandHandler))]
	class PassoverCommand : ICommand
	{
		public bool SanitizeResponse => false;
		public string Command => "passover";

		public string[] Aliases => new string[] { "po" };

		public string Description => "Kill your firstborn son and gain his health.";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "";
			if (Plugin.Instance.Config.AllowPassover)
			{
				Player player = Player.Get(((CommandSender)sender).SenderId);
				if (player.Role.Type == RoleTypeId.Scp049)
				{
					if (TimerFunction())
					{
						string firstborn = Plugin.Instance.PlayerHandlers.DoctorsZombies[player.UserId].First();
						Player zombie = Exiled.API.Features.Player.List.Where(z => z.UserId == firstborn).First();
						if (firstborn != null && zombie != null)
						{
							float tempHealth = zombie.Health;
							zombie.Kill(Exiled.API.Enums.DamageType.Decontamination);
							player.Heal(tempHealth, true);
							response = $"You rejected the Israelites gained {tempHealth} for sacrificing your firstborn Egyptian son.";
						}
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
