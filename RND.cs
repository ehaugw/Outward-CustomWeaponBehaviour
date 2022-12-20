using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomWeaponBehaviour
{
    //public void Play(string stateName, [UnityEngine.Internal.DefaultValue("-1")] int layer, [UnityEngine.Internal.DefaultValue("float.NegativeInfinity")] float normalizedTime)

    //[HarmonyLib.HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(string), typeof(int), typeof(float)})]
    //public class Animator_Play
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Animator __instance, string stateName, int layer)
    //    {
    //    }
    //}

    //[HarmonyLib.HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(int), typeof(int)})]
    //public class Animator_Play2
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Animator __instance, int stateNameHash, int layer)
    //    {
    //    }
    //}

    //[HarmonyLib.HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(int) })]
    //public class Animator_Play3
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Animator __instance, int stateNameHash)
    //    {
    //    }
    //}


    //[HarmonyLib.HarmonyPatch(typeof(Character), "SpellCast")]
    //public class Character_SpellCast
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(Character __instance)
    //    {
    //    }
    //}

    //[HarmonyLib.HarmonyPatch(typeof(Character), "HitStarted")]
    //public class Character_HitStarted
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(Character __instance, ref int _attackID)
    //    {
    //        if (_attackID != -2)
    //        {
    //            if (BehaviourManager.SpellHasAttackAnimation(__instance))
    //            {
    //                //At.Call(__instance, "SpellCast", new object[] { });
    //            }
    //        }
    //    }
    //}




    ////Block related stuff.Not in use due to visual glitching.
    //[HarmonyLib.HarmonyPatch(typeof(Character), "SendBlockStateTrivial", new Type[] { typeof(bool) })]
    //public class Character_SendBlockStateTrivial
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Character __instance, ref bool _active)
    //    {
    //        int type = 0;
    //        int id = 0;
    //        BehaviourManager.AdaptGrip(__instance, ref type, ref id);
    //    }
    //}

    //[HarmonyLib.HarmonyPatch(typeof(Character), "StartBlocking")]
    //public class Character_StartBlocking
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Character __instance)
    //    {
    //        int type = 0;
    //        int id = 0;
    //        BehaviourManager.AdaptGrip(__instance, ref type, ref id);
    //    }
    //}

    //[HarmonyLib.HarmonyPatch(typeof(Character), "StopBlocking")]
    //public class Character_StopBlocking
    //{
    //    [HarmonyPrefix]
    //    public static void Prefix(Character __instance)
    //    {
    //        if (__instance.Blocking)
    //        {
    //            CustomWeaponBehaviour.ResetGrip(__instance);
    //        }
    //    }
    //}


    //[HarmonyLib.HarmonyPatch(typeof(Character), "NextAttack")] //Fix to light-heavy-heavy bug
    //public class Character_NextAttack
    //{
    //    [HarmonyPrefix]
    //    public static bool Prefix(Character __instance, ref int _type, ref int _id)
    //    {
    //        if (__instance.NextAttack1Only && _type != 0)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //}
}
