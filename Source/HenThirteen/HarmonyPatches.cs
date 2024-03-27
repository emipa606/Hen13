using System.Reflection;
using HarmonyLib;
using Verse;

namespace HenThirteen;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("Mlie.HenThirteen").PatchAll(Assembly.GetExecutingAssembly());
    }
}