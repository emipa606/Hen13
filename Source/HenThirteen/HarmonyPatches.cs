﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace HenThirteen
{
    [StaticConstructorOnStartup]
    internal static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            new Harmony("Mlie.HenThirteen").PatchAll();
        }

        [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
        public static class IncidentWorker_RaidEnemy_TryExecuteWorker_Prefix
        {
            public static bool Prefix(IncidentParms parms)
            {
                if (Find.StoryWatcher.statsRecord.numRaidsEnemy != 1)
                {
                    return true;
                }

                var map = (Map) parms.target;
                if (!RCellFinder.TryFindRandomPawnEntryCell(out var result, map, CellFinder.EdgeRoadChance_Animal))
                {
                    return true;
                }

                foreach (var p in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer))
                {
                    if (p.kindDef == DefDatabase<PawnKindDef>.GetNamed("HenThirteen"))
                    {
                        return true;
                    }
                }

                var loc = CellFinder.RandomClosewalkCellNear(result, map, 12);
                var kind = DefDatabase<PawnKindDef>.GetNamed("HenThirteen");
                var pawn = PawnGenerator.GeneratePawn(kind);
                GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
                //PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, "Hen 13");
                pawn.Name = new NameTriple("Hen", "Hero Chicken", "13");
                pawn.SetFaction(Faction.OfPlayer);
                Find.LetterStack.ReceiveLetter("Hen 13 Joins",
                    "A mythical hero of wide renown has sensed your danger and has decided to join your cause.  Hen Thirteen is on the scene",
                    LetterDefOf.PositiveEvent, new TargetInfo(result, map));

                return false;
            }
        }
    }
}