using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using RemoteAdmin;
using UnityEngine;

namespace ZombieSuicideHotline
{
	[CommandHandler(typeof(ClientCommandHandler))]
	class StuckCommmand : ICommand
	{
		public string Command => "Stuck";

		public string[] Aliases => new string[] { "unstuck" };

		public string Description => "Frees stuck players";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "";
			if (Plugin.Instance.Config.AllowUnstuck)
			{
				Player player = Player.Get(((CommandSender)sender).SenderId);
				//if (player.Team == Team.SCP)
				//{
					Timing.RunCoroutine(UnstuckPlayer(player,player.CurrentRoom));
					response = "Unstuck started, wait " + Plugin.Instance.Config.UnstuckTime + " seconds";
				//}
				//else
				//{
					//response = "You must be a SCP to use this command!";
				//}
			}
			else
			{
				response = ".stuck is not enabled.";
				return false;
			}
			return true;
		}

		public IEnumerator<float> UnstuckPlayer(Player player, Room room)
		{
			int timer = 0;
			bool leftroom = false;
			while (timer < Plugin.Instance.Config.UnstuckTime)
			{
				yield return Timing.WaitForSeconds(10);
				timer += 10;
				if (player.CurrentRoom != room)
				{
					player.Broadcast(5, "You have left the room, unstuck canceled.");
					leftroom = true;
					break;
				}
			}
			if (!leftroom)
			{
				player.Position = player.Role.Type.GetRandomSpawnLocation().Position;
			}
		}

	}
}
