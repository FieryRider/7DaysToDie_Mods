using DMT;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class FovChanger : IHarmony
{
    protected static float fov = 65.0f;
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
            __instance.RenderingFieldOfView = fov;
            __instance.originalRenderingFieldOfView = fov;
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
                ___m_WeaponCamera.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(___m_WeaponCamera.gameObject.GetComponent<Camera>().fieldOfView, fov, num * 15f);
            }

            return false;
        }
    }
}
