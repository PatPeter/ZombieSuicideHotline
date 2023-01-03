using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSuicideHotline.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	class PerksCommand : ICommand
	{
		public string Command => "perks";

		public string[] Aliases => new string[] { };

		public string Description => "";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Not currently implemented.";
			return true;
		}
	}
}
