using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using UnityEngine;

namespace ZombieSuicideHotline
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class RecallCommand : ICommand
    {
        public string Command => "recall";

        public string[] Aliases => null;

        public string Description => "Allows you to bring all zombies to you as SCP 049";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "";
            Player player = Player.Get(((CommandSender)sender).SenderId);
                if (player.Role == RoleType.Scp049)
                {
                    if (Timerfunc())
                    {
                        foreach (Player players in Exiled.API.Features.Player.List)
                        {
                            if (players.Role == RoleType.Scp0492 && Plugin.Singleton.PlayerHandlers.DoctorsZombies[player.UserId].Contains(players.UserId))
                            {
                                players.Position = player.Position;
                                response = "Zombies recalled!";
                            }
                        }
                    }
                    else
                    {
                        response = "Recall is on cooldown for " + (Lasttime + Plugin.Singleton.Config.RecallCooldown - Time.time).ToString();
                    }
                    if (response == "")
                    {
                        response = "No alive Zombies!";
                    }
                }
                else
                {
                    response = "You must be SCP 049 to use this command!";
                }
            return true;
        }
        public float Lasttime = 0;
        public bool Timerfunc()
        {
            if (Lasttime + Plugin.Singleton.Config.RecallCooldown < Time.time)
            {
                Lasttime = Time.time;
                return true;
            }
            return false;
        }
    }
}
