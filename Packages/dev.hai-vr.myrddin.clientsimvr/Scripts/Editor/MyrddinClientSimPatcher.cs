using System.IO;
using System.Reflection;
using Hai.Myrddin.ClientSimVR.Runtime;
using HarmonyLib;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.ClientSim;

namespace Hai.Myrddin.ClientSimVR.Editor
{
    [InitializeOnLoad]
    public class MyrddinClientSimPatcher
    {
        private const string HarmonyIdentifier = "dev.hai-vr.myrddin.clientsimvr.Harmony";
        private static readonly string PrefabPath = Path.Combine("MyrddinClientSimVR", "ClientSimMyrddinTrackingData");
        
        private static readonly Harmony Harm;

        static MyrddinClientSimPatcher()
        {
            Harm = new Harmony(HarmonyIdentifier);

            AddClientSimVRInitializer();
        }

        private static void AddClientSimVRInitializer()
        {
            var clientSimPlayerToPatch = typeof(ClientSimPlayer);
            var theInitializer = clientSimPlayerToPatch.GetMethod("Initialize", BindingFlags.Instance | BindingFlags.Public);
            var ourPatch = typeof(MyrddinClientSimPatcher).GetMethod(nameof(PatchInitialize));
            
            DoPatch(theInitializer, new HarmonyMethod(ourPatch));
        }

        public static bool PatchInitialize(ClientSimPlayer __instance)
        {
            if (PlayerPrefs.GetInt("Hai.Myrddin.ClientSimVR") > 0)
            {
                var myrddinTrackingData = Resources.Load<GameObject>(PrefabPath).GetComponent<ClientSimMyrddinTrackingProvider>();
                
                var playerTrackingDataField = Field("playerTrackingData");
                (playerTrackingDataField.GetValue(__instance) as ClientSimTrackingProviderBase).gameObject.SetActive(false);
                myrddinTrackingData.gameObject.SetActive(true);
                (Field("reticle").GetValue(__instance) as ClientSimReticle).gameObject.SetActive(false);
                playerTrackingDataField.SetValue(__instance, myrddinTrackingData);
                
                Debug.Log("(MyrddinClientSimPatcher) ClientSimVR is ON: Successfully switched to MyrddinTrackingData");
            }
            else
            {
                Debug.Log("(MyrddinClientSimPatcher) ClientSimVR is switched OFF");
            }
            
            return true;
        }

        private static FieldInfo Field(string name)
        {
            return typeof(ClientSimPlayer).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static void DoPatch(MethodInfo source, HarmonyMethod destination)
        {
            Harm.Patch(source, destination);
        }
    }
}