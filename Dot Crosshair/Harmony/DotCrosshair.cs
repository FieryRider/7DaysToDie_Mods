using DMT;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

[HarmonyPatch(typeof(EntityPlayerLocal))]
[HarmonyPatch("guiDrawCrosshair")]
public class DotCrosshair : IHarmony
{
    public void Start()
    {
        Debug.Log("Loading Patch: " + GetType().ToString());
        Harmony harmony = new Harmony(GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    static readonly int CrosshairSize = 2;
    static Color CrosshairColor = new Color(0.8f, 0.8f, 0.8f);

    static bool Prefix(EntityPlayerLocal __instance, NGUIWindowManager ___nguiWindowManager, NGuiWdwInGameHUD _guiInGame, bool bModalWindowOpen)
    {
        if (!_guiInGame.showCrosshair)
            return false;
        if (Event.current.type != EventType.Repaint)
            return false;
        if (__instance.IsDead())
            return false;
        if (__instance.AttachedToEntity != null)
            return false;

        ItemClass.EnumCrosshairType crosshairType = __instance.inventory.holdingItem.GetCrosshairType(__instance.inventory.holdingItemData);
        if (crosshairType == ItemClass.EnumCrosshairType.CrosshairOnAiming || crosshairType == ItemClass.EnumCrosshairType.Crosshair)
        {
            if (bModalWindowOpen) return false;
            ___nguiWindowManager.Show(EnumNGUIWindow.StealthIndicator, false);
            if (__instance.inventory == null) return false;
            Vector2 crosshairPosition2D = __instance.GetCrosshairPosition2D();
            crosshairPosition2D.y = (float)Screen.height - crosshairPosition2D.y;

            if (!__instance.AimingGun || ItemActionAttack.ShowDistanceDebugInfo)
                DrawFilledCircle4PointCenter(crosshairPosition2D, CrosshairSize, CrosshairColor);

            return false;
        }
        else
        {
            return true;
        }
        return true;
    }

    /* Modified Bresenham's circle drawing algorithm for 4-point center instead of 1-point center */
    static void DrawFilledCircle4PointCenter(Vector2 centerUpperLeftCorner, int radius, Color color)
    {
        int x = 0;
        int y = radius;
        float decision = 3 - 2 * radius;
        DrawPartOfFilledCircle((int)centerUpperLeftCorner.x, (int)centerUpperLeftCorner.y, x, y, color, true);
        while (y >= x)
        {
            x++;
            if (decision > 0)
            {
                y--;
                decision += 4 * (x - y) + 10;
            }
            else
            {
                decision += 4 * x + 6;
            }
            DrawPartOfFilledCircle((int)centerUpperLeftCorner.x, (int)centerUpperLeftCorner.y, x, y, color, true);;
        }
    }

    static void DrawPartOfFilledCircle(int cx, int cy, int x, int y, Color color, bool drawArround4PointCenter = false)
    {
        if (drawArround4PointCenter)
        {
            int rightCenterX = cx;
            int upperCenterY = cy;
            int leftCenterX = rightCenterX - 1;
            int lowerCenterY = upperCenterY - 1;
            GUIUtils.DrawLine(new Vector2(leftCenterX - x, upperCenterY + y), new Vector2(rightCenterX + x, upperCenterY + y), color);
            GUIUtils.DrawLine(new Vector2(leftCenterX - x, lowerCenterY - y), new Vector2(rightCenterX + x, lowerCenterY - y), color);
            GUIUtils.DrawLine(new Vector2(leftCenterX - y, upperCenterY + x), new Vector2(rightCenterX + y, upperCenterY + x), color);
            GUIUtils.DrawLine(new Vector2(leftCenterX - y, lowerCenterY - x), new Vector2(rightCenterX + y, lowerCenterY - x), color);
        }
        else
        {
            GUIUtils.DrawLine(new Vector2(cx - x, cy + y), new Vector2(cx + x, cy + y), color);
            GUIUtils.DrawLine(new Vector2(cx - x, cy - y), new Vector2(cx + x, cy - y), color);
            GUIUtils.DrawLine(new Vector2(cx - y, cy + x), new Vector2(cx + y, cy + x), color);
            GUIUtils.DrawLine(new Vector2(cx - y, cy - x), new Vector2(cx + y, cy - x), color);
        }
    }
}
