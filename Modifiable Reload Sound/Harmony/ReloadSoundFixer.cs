using DMT;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ReloadSoundFixer
{
    public class ReloadSoundFixer_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log("Loading Patch: " + GetType().ToString());
            Harmony harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(ItemClass))]
    [HarmonyPatch("CloneModel")]
    [HarmonyPatch(new Type[] { typeof(World), typeof(ItemValue), typeof(Vector3), typeof(Transform), typeof(bool), typeof(bool), typeof(long) })]
    public class SoundReloadSetter
    {
        static Transform Postfix(Transform __result, ItemClass __instance)
        {
            if (__result == null)
                return __result;

            GameObject gameObject = __result.gameObject;
            if (__instance.Properties.Contains("ReloadAnimationClip") && __instance.Properties.Contains("SoundReloadClip0"))
            {
                if (gameObject.GetComponent<AnimationEventBridge>() == null)
                    gameObject.AddComponent<AnimationEventBridge>();

                RuntimeAnimatorController runtimeAnimatorController = gameObject.GetComponent<Animator>().runtimeAnimatorController;
                foreach (AnimationClip animationClip in runtimeAnimatorController.animationClips)
                {
                    if (__instance.Properties.GetString("ReloadAnimationClip").Equals(animationClip.name))
                    {
                        bool hasSoundEvents = false;
                        foreach (AnimationEvent animationEvent in animationClip.events)
                        {
                            if ("PlaySound".Equals(animationEvent.functionName))
                            {
                                hasSoundEvents = true;
                                break;
                            }
                        }
                        if (hasSoundEvents) break;

                        int i = 0;
                        while (__instance.Properties.Contains("SoundReloadClip" + i))
                        {
                            string soundReloadClip = __instance.Properties.GetString("SoundReloadClip" + i);
                            if (!string.Empty.Equals(soundReloadClip))
                            {
                                float soundClipOffset = float.Parse(__instance.Properties.Params1["SoundReloadClip" + i]);
                                AnimationEvent animationEvent = new AnimationEvent
                                {
                                    functionName = "PlaySound",
                                    stringParameter = soundReloadClip,
                                    time = soundClipOffset
                                };
                                animationClip.AddEvent(animationEvent);
                            }
                            i++;
                        }
                        break;
                    }
                }
            }
            return __result;
        }
    }

    [HarmonyPatch(typeof(AnimatorRangedReloadState))]
    [HarmonyPatch("OnStateEnter")]
    [HarmonyPatch(new Type[] { typeof(Animator), typeof(AnimatorStateInfo), typeof(int) })]
    public class CharacterSoundReloadRemover
    {
        static bool Prefix(Animator animator, AnimatorRangedReloadState __instance, ItemActionRanged ___actionRanged)
        {
            // From the beginning of the original function
            animator.SetBool("Reload", false);
            EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
            if (componentInParent == null)
                return false;

            // My code
            AnimationClip[] animationClips = animator.runtimeAnimatorController.animationClips;
            foreach (AnimationClip animationClip in animationClips)
            {
                List<string> fpvAnimationClipNames = new List<string> { "fpvPistolReload", "fpvMagnumReload", "fpvDesertEagleReload", "fpvMP5Reload", "fpvBlunderbussReload",
                 "fpvSawedOffShotgunReload", "fpvDoubleBarrelShotgunReload", "fpvAutoShotgunReload", "fpvHuntingRifleReload", "fpvSniperRifleReload", "fpvSharpShooterRifleReload",
                 "fpvAK47Reload", "fpvTacticalAssaultRifleReload", "fpvM60Reload", "fpvM136Reload"};
                 //fpvJunkTurretReload removed because there is no item animation

                if (fpvAnimationClipNames.Contains(animationClip.name))
                    animationClip.events = new AnimationEvent[0];
            }
            return true;
        }
    }
}