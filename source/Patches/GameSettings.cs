
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using TownOfUs.CustomOption;
using UnityEngine;
using Reactor.Extensions;

namespace TownOfUs {
    
    [HarmonyPatch]
    public static class GameSettings
    {

        public static bool AllOptions = false;
        
        /*public static string StringBuild()
        {
            var builder = new StringBuilder("Roles:\n");
            foreach (var option in TownOfUs.Roles)
            {
                builder.AppendLine($"     {option.Name}: {option}");
            }

            builder.AppendLine("Modifiers:");
            foreach (var option in TownOfUs.Modifiers)
            {
                builder.AppendLine($"     {option.Name}: {option}");
            }
            
            
            foreach (var option in TownOfUs.AllOptions)
            {
                builder.AppendLine($"{option.Name}: {option}");
            }
            

            return builder.ToString();
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        public static class LobbyFix
        {

            public static bool Prefix()
            {
                
                DestroyableSingleton<HudManager>.Instance.GameSettings.Text = StringBuild();
                DestroyableSingleton<HudManager>.Instance.GameSettings.gameObject.SetActive(true);
                return false;
            }
        }


        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyAfter("com.comando.essentials")]
        public static class FixScale
        {
            public static void Prefix(HudManager __instance)
            {
                __instance.GameSettings.scale = 0.3f;
            }
        }*/

        [HarmonyPatch] //ToHudString
        private static class GameOptionsDataPatch
        {
            
            public static IEnumerable<MethodBase> TargetMethods()
            {
                return typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
            }

            private static void Postfix(ref string __result)
            {              
                StringBuilder builder = new StringBuilder(AllOptions ? __result : "");
                
                foreach (CustomOption.CustomOption option in CustomOption.CustomOption.AllOptions)
                {
                    if (option.Name == "Custom Game Settings" && !AllOptions) break;
                    if (option.Type == CustomOptionType.Button) continue;
                    if (option.Type == CustomOptionType.Header) builder.AppendLine($"\n{option.Name}");
                    else if(option.Indent) builder.AppendLine($"     {option.Name}: {option}");
                    else builder.AppendLine($"{option.Name}: {option}");
                }

                __result = builder.ToString();

                int num = !CustomOption.CustomOption.LobbyTextScroller ? 0 : (__result.Count(c => c == '\n') > 38 ? 1 : 0);
                __result = num == 0 ? __result.Insert(__result.IndexOf('\n'), "Press Tab to see All Options") : __result.Insert(__result.IndexOf('\n'), " (Scroll for more)");
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.FixedUpdate))]
        private static class LobbyBehaviourPatch
        {
            private static void Postfix()
            {
                if (Input.GetKeyInt(KeyCode.Tab))
                {
                    AllOptions = !AllOptions;
                }

                HudManager.Instance.GameSettings.fontSize = 1.3f;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update {
            public static void Postfix(ref GameOptionsMenu __instance) {
                __instance.GetComponentInParent<Scroller>().YBounds.max = 70f;
            }
        }
    }
}