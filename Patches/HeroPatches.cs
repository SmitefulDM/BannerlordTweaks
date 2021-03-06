﻿using HarmonyLib;
using ModLib.Debugging;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordTweaks.Patches
{
    [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class AddSkillXpPatch
    {
        private static double GetMultiplier(int skillLevel)
        {
            if (Settings.Instance.HeroSkillExperienceOverrideMultiplierEnabled)
                return Settings.Instance.HeroSkillExperienceOverrideMultiplier;
            else
                return Math.Max(1, 0.0315769 * Math.Pow(skillLevel, 1.020743));
        }

        static bool Prefix(Hero __instance, SkillObject skill, int xpAmount)
        {
            try
            {
                HeroDeveloper hd = (HeroDeveloper)(typeof(Hero).GetField("_heroDeveloper", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance));
                if (hd != null)
                {
                    if (xpAmount > 0)
                    {
                        double multiplier = GetMultiplier(__instance.GetSkillValue(skill));
                        int newXpAmount = (int)Math.Ceiling(xpAmount * multiplier);
                        hd.AddSkillXp(skill, newXpAmount);
                    }
                    else
                        hd.AddSkillXp(skill, xpAmount);
                }
            }
            catch (Exception ex)
            {
                ModDebug.ShowError($"An exception occurred whilst trying to apply the hero xp multiplier.", "", ex);
            }
            return false;
        }

        static bool Prepare()
        {
            return Settings.Instance.HeroSkillExperienceMultiplierEnabled;
        }
    }
}
