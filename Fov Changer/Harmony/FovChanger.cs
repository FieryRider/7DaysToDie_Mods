using DMT;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection.Emit;

public class FovChanger : IHarmony
{
    public void Start()
    {
        Debug.Log("Loading Patch: " + GetType().ToString());
        Harmony harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(vp_FPWeapon))]
    [HarmonyPatch("Start")]
    public class FovValueChanger
    {
        static void Postfix(vp_FPWeapon __instance)
        {
            // I don't actually use these values because the game overrides them for non-gun items. RenderingFieldOfView is set to 45 from scripts inside resources.assets such as WeaponBow, WeaponDefault ...
            CustomPrefs customPrefs = CustomPrefs.GetCustomPrefs();
            try
            {
                __instance.RenderingFieldOfView = customPrefs.GetCustomPref<int>("WeaponFov");
                __instance.originalRenderingFieldOfView = customPrefs.GetCustomPref<int>("WeaponFov");
            }
            catch (ArgumentException e)
            {
                customPrefs.SetCustomPref<int>("WeaponFov", 60);
                __instance.RenderingFieldOfView = customPrefs.GetCustomPref<int>("WeaponFov");
                __instance.originalRenderingFieldOfView = customPrefs.GetCustomPref<int>("WeaponFov");
            }
        }
    }

    [HarmonyPatch(typeof(vp_FPWeapon))]
    [HarmonyPatch("UpdateZoom")]
    public class FovCheckRemover
    {
        static bool Prefix(vp_FPWeapon __instance, float ___m_FinalZoomTime, bool ___m_Wielded, GameObject ___m_WeaponCamera)
        {
            if (!___m_Wielded)
                return false;

            __instance.RenderingZoomDamping = Mathf.Max(__instance.RenderingZoomDamping, 0.01f);
            float num = 1f - (___m_FinalZoomTime - Time.time) / __instance.RenderingZoomDamping;
            if (___m_WeaponCamera != null)
            {
                CustomPrefs customPrefs = CustomPrefs.GetCustomPrefs();
                ___m_WeaponCamera.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(___m_WeaponCamera.gameObject.GetComponent<Camera>().fieldOfView, customPrefs.GetCustomPref<int>("WeaponFov"), num * 15f);
            }

            return false;
        }
    }
}
