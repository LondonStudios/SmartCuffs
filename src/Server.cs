using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
namespace Server
{
    public class Main : BaseScript
    {
        public Main()
        {
            EventHandlers["Server:CuffSound"] += new Action<int, float, string, float>((networkId, soundRadius, soundFile, soundVolume) =>
            {
                TriggerClientEvent("Client:CuffSound", networkId, soundRadius, soundFile, soundVolume);
            });

            // Types: 0 = Rearcuff, 1 = Frontcuff, 3 = Uncuff
            EventHandlers["Server:CuffLocal"] += new Action<int, int, Vector3, int, int>((request, client, location, type, targetId) =>
            {
                TriggerClientEvent("Client:CuffLocal", request, client, location, type, targetId);
            });

            EventHandlers["Server:ReturnResult"] += new Action<int, int>((request, requestType) =>
            {
                TriggerClientEvent("Client:ReturnResult", request, requestType);
            });
        }
    }
}
