using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using SharpConfig;

namespace SmartCuff
{
    public class Main : BaseScript
    {
        public int pairs = 1;
        public bool cuffed { get; set; }
        public int cuffType { get; set; }

        public int cuffs;

        public int playerLoop = 64;

        public int cuffedBy = 0;

        public bool permissionsEnabled = false;

        public bool useId = false;

        public Main()
        {
            ReadConfig();

            RequestAnim("anim@move_m@prisoner_cuffed");
            RequestAnim("mp_arresting");

            RequestModel(GetHashKey("p_cs_cuffs_02_s"));
            
            TriggerEvent("chat:addSuggestion", "/resetcuff", "Gives you a pair of handcuffs again");


            if (GetConvar("onesync_enabled", "true") == "true")
            {
                playerLoop = 256;
            }

            EventHandlers["Client:CuffSound"] += new Action<int, float, string, float>((networkId, soundRadius, soundFile, soundVolume) =>
            {
                Vector3 playerCoords = Game.Player.Character.Position;
                Vector3 targetCoords = GetEntityCoords(NetworkGetEntityFromNetworkId(networkId), true);
                var distance = Vdist(playerCoords.X, playerCoords.Y, playerCoords.Z, targetCoords.X, targetCoords.Y, targetCoords.Z);
                float distanceVolumeMultiplier = (soundVolume / soundRadius);
                float distanceVolume = soundVolume - (distance * distanceVolumeMultiplier);
                if (distance <= soundRadius)
                {
                    SendNuiMessage(string.Format("{{\"submissionType\":\"cuffSound\", \"submissionVolume\":{0}, \"submissionFile\":\"{1}\"}}", (object)distanceVolume, (object)soundFile));
                }
            });

            // Types: 0 = Rearcuff, 1 = Frontcuff, 3 = Uncuff
            EventHandlers["Client:CuffLocal"] += new Action<int, int, Vector3, int>((request, client, location, type) =>
            {
                var local = GetEntityCoords(PlayerPedId(), true);
                if (local.DistanceToSquared(location) < 5.5f && GetPlayerServerId(PlayerId()) == request)
                {
                    if (cuffed)
                    {
                        TriggerServerEvent("Server:CuffSound", Game.Player.Character.NetworkId, 20.0f, "cuff", 0.6f);
                        TriggerServerEvent("Server:ReturnResult", cuffedBy, 1);
                        cuffed = false;
                    }
                    else
                    {
                        if (type != 3)
                        {
                            TriggerServerEvent("Server:CuffSound", Game.Player.Character.NetworkId, 20.0f, "cuff", 0.6f);
                            cuffedBy = client;
                            TriggerServerEvent("Server:ReturnResult", client, 0);
                            cuffed = true;
                            cuffType = type;
                            ProcessCuffs(type);
                        }  
                    }
                }
            });

            // Types: 0 = Remove, 1 = Add, 2 = Passcuffs
            EventHandlers["Client:ReturnResult"] += new Action<int, int>((request, requestType) =>
            {
                if (GetPlayerServerId(PlayerId()) == request)
                {
                    if (requestType == 0)
                    {
                        pairs--;
                        if (pairs < 0)
                        {
                            pairs = 0;
                        }
                    }
                    else if (requestType == 1)
                    {
                        pairs++;
                    }
                }
                else if (requestType == 2)
                {
                    if (GetPlayerServerId(PlayerId()) == request)
                    {
                        pairs++;
                        Screen.ShowNotification("You have been passed a pair of ~r~handcuffs.");
                    }
                }
            });
        }

        private void ProcessCuffs(int type)
        {
            string animName = "";
            string animDict = "";
            if (type == 0)
            {
                animName = "idle";
                animDict = "mp_arresting";
            }
            else if (type == 1)
            {
                
                animName = "idle";
                animDict = "anim@move_m@prisoner_cuffed";
            }
            ContinueCuffs(animDict, animName, type);
        }

        private async void ContinueCuffs(string animDict, string animName, int type)
        {
            var player = PlayerPedId();
            SetEnableHandcuffs(player, true);
            SetPedCanPlayGestureAnims(player, false);
            SetPedPathCanUseLadders(player, false);
            DisableKeybinds();
            ProcessModel(false, type);
            while (cuffed)
            {
                if (!IsEntityPlayingAnim(player, animDict, animName, 49) || IsPedRagdoll(player))
                {
                    TaskPlayAnim(player, animDict, animName, 8.0f, -8, -1, 49, 0, false, false, false);
                }
                await Delay(500);
            }
            ClearPedTasks(player);
            SetPedCanPlayGestureAnims(player, true);
            SetPedPathCanUseLadders(player, true);
            SetEnableHandcuffs(player, false);
            ProcessModel(true, type);
            UncuffPed(player);
        }

        private void ProcessModel(bool remove, int type)
        {
            if (remove)
            {
                DetachEntity(cuffs, true, true);
                DeleteEntity(ref cuffs);
            }
            else
            {
                var coords = GetEntityCoords(PlayerPedId(), false);
                cuffs = CreateObject(GetHashKey("p_cs_cuffs_02_s"), coords.X, coords.Y, coords.Z, true, true, true);
                var networkId = ObjToNet(cuffs);
                SetNetworkIdExistsOnAllMachines(networkId, true);
                SetNetworkIdCanMigrate(networkId, false);
                NetworkSetNetworkIdDynamic(networkId, true);
                if (type == 0)
                {
                    AttachEntityToEntity(cuffs, GetPlayerPed(PlayerId()), GetPedBoneIndex(GetPlayerPed(PlayerId()), 60309), -0.055f, 0.06f, 0.04f, 265.0f, 155.0f, 80.0f, true, false, false, false, 0, true);
                }
                else if (type == 1)
                {
                    AttachEntityToEntity(cuffs, GetPlayerPed(PlayerId()), GetPedBoneIndex(GetPlayerPed(PlayerId()), 60309), -0.058f, 0.005f, 0.090f, 290.0f, 95.0f, 120.0f, true, false, false, false, 0, true);
                }
            }
        }

        private async void RequestModel(int model)
        {
            while (!HasModelLoaded((uint)model))
            {
                await Delay(0);
            }
            await Delay(100);
        }

        private async void DisableKeybinds()
        {
            while (cuffed)
            {
                DisableControlAction(0, 140, true);
                DisableControlAction(0, 141, true);
                DisableControlAction(0, 257, true);
                DisableControlAction(0, 142, true);
                DisableControlAction(0, 25, true);
                DisableControlAction(0, 24, true);
                DisableControlAction(0, 69, true);
                DisableControlAction(0, 92, true);
                DisableControlAction(0, 114, true);
                await Delay(0);
            }
            await Delay(0);
        }

        private bool IsEligible()
        {
            if (cuffed || IsPedInAnyVehicle(PlayerPedId(), true))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Command("resetcuff")]
        private void ResetCuffs()
        {
            pairs = 1;
            Screen.ShowNotification("You have one pair of handcuffs again.");
        }

        private void ProcessSubmission(int type)
        {
            if (IsEligible())
            {
                var target = Raycast();
                if (IsEntityAPed(target))
                {
                    var server = GetPlayerServerId(GetPlayer(target));
                    if (!(server == -1))
                    {
                        if (pairs > 0)
                        {
                            TriggerServerEvent("Server:CuffLocal", server, GetPlayerServerId(PlayerId()), GetEntityCoords(PlayerPedId(), true), type, 1);
                        }
                        else
                        {
                            TriggerServerEvent("Server:CuffLocal", server, GetPlayerServerId(PlayerId()), GetEntityCoords(PlayerPedId(), true), 3, 1);
                        }
                    }
                }
            }  
        }

        private void ProcessSubmissionId(int type, int id)
        {
            if (IsEligible())
            {
                var server = id;
                if (!(server == -1))
                {
                    if (pairs > 0)
                    {
                        TriggerServerEvent("Server:CuffLocal", server, GetPlayerServerId(PlayerId()), GetEntityCoords(PlayerPedId(), true), type, 1);
                    }
                    else
                    {
                        TriggerServerEvent("Server:CuffLocal", server, GetPlayerServerId(PlayerId()), GetEntityCoords(PlayerPedId(), true), 3, 1);
                    }
                }
            }
        }

        private int Raycast()
        {
            var location = GetEntityCoords(PlayerPedId(), true);
            var offSet = GetOffsetFromEntityInWorldCoords(PlayerPedId(), 0.0f, 2.0f, 0.0f);
            var shapeTest = StartShapeTestCapsule(location.X, location.Y, location.Z, offSet.X, offSet.Y, offSet.Z, 1.0f, 12, PlayerPedId(), 7);
            bool hit = false;
            Vector3 endCoords = new Vector3(0f, 0f, 0f);
            Vector3 surfaceNormal = new Vector3(0f, 0f, 0f);
            int entityHit = 0;
            var result = GetShapeTestResult(shapeTest, ref hit, ref endCoords, ref surfaceNormal, ref entityHit);
            return entityHit;
        }

        private int GetPlayer(int player)
        {
            for (int i = 0; i < playerLoop; i++)
            {
                if (GetPlayerPed(i) == player)
                {
                    return i;
                }
            }
            return -1;
        }

        private void ReadConfig()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");
            if (Configuration.LoadFromString(data).Contains("SmartCuffs", "PermissionsEnabled") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                permissionsEnabled = loaded["SmartCuffs"]["PermissionsEnabled"].BoolValue;

                useId = loaded["SmartCuffs"]["UsePlayerIds"].BoolValue;

                CreateCommand(permissionsEnabled);
            }
        }

        private void CreateCommand(bool permissions)
        {
            RegisterCommand("frontcuff", new Action<string, List<object>, string>((source, args, raw) =>
            {
                if (useId)
                {
                    int argsInt = 0;
                    Int32.TryParse(Convert.ToString(args[0]), out argsInt);

                    if (GetPlayerServerId(PlayerId()) == argsInt)
                    {
                        Screen.ShowNotification("You are not able to cuff yourself.");
                    }
                    else
                    {
                        ProcessSubmissionId(1, argsInt);
                    }
                }
                else
                {
                    ProcessSubmission(1);
                }
            }), permissions);



            RegisterCommand("cuff", new Action<string, List<object>, string>((source, args, raw) =>
            {
                if (useId)
                {
                    int argsInt = 0;
                    Int32.TryParse(Convert.ToString(args[0]), out argsInt);

                    if (GetPlayerServerId(PlayerId()) == argsInt)
                    {
                        Screen.ShowNotification("You are not able to cuff yourself.");
                    }
                    else
                    {
                        ProcessSubmissionId(0, argsInt);
                    }
                }
                else
                {
                    ProcessSubmission(0);
                }
            }), permissions);

            if (!useId)
            {
                TriggerEvent("chat:addSuggestion", "/frontcuff", "Frontcuff the nearest player");
                TriggerEvent("chat:addSuggestion", "/cuff", "Rear cuff the nearest player");

                RegisterKeyMapping("frontcuff", "Front Handcuff", "keyboard", ",");
                RegisterKeyMapping("cuff", "Rear Handcuff", "keyboard", ".");

            }
            else
            {
                TriggerEvent("chat:addSuggestion", "/frontcuff", "Frontcuff another player", new[]
                {
                    new { name="PlayerID", help="Target Player ID" },
                });

                TriggerEvent("chat:addSuggestion", "/cuff", "Rearcuff another player", new[]
                {
                    new { name="PlayerID", help="Target Player ID" },
                });
            }
        }

        private async void RequestAnim(string dictionary)
        {
            RequestAnimDict(dictionary);
            while (!HasAnimDictLoaded(dictionary))
            {
                await Delay(100);
            }
            await Delay(0);
        }
    }
}
