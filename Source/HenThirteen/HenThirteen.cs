using System;
using HarmonyLib;
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

    public class ThingDef_HenThirteen : ThingDef
    {
        public float ZerkFactor = 0.5f;
    }

    public class HenThirteen : Pawn
    {
        private ThingDef_HenThirteen Def =>
            //Case sensitive! If you use Def it will return Def, which is this getter. This will cause a never ending cycle and a stack overflow.
            def as ThingDef_HenThirteen;

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);

            if (Faction != Faction.OfPlayer)
            {
                //Log.Message("not a player chicken 13");
                return;
            }

            if (absorbed)
            {
                return;
            }

            var DamageAbsorbed = 0f;
            var hediffSet = health.hediffSet;

            foreach (var hediff in hediffSet.hediffs)
            {
                if (hediff is Hediff_Injury)
                {
                    DamageAbsorbed += hediff.Severity;
                }
            }

            var PercentHealth = Math.Min(DamageAbsorbed / (HealthScale * 25.0f), 0.99f);
            var rand = Rand.Value;
            //Log.Message("Not yet absorbed", true);
            //Log.Message(PercentHealth + " " + DamageAbsorbed + " " + HealthScale + " " + rand, true);
            absorbed = rand <= PercentHealth * Def.ZerkFactor;
            //Log.Message("2nd chance absord result " + absorbed + "    " + (PercentHealth * Def.ZerkFactor));

            if (absorbed)
            {
                Messages.Message(
                    "Chicken 13's heroically shrugs off damage from " + dinfo.Instigator.LabelShort + "'s " +
                    dinfo.Weapon.label, MessageTypeDefOf.PositiveEvent);
            }
        }
    }

    //public class IncidentWorker_RaidEnemy_HenThirteen : IncidentWorker_RaidEnemy
    //{
    //    protected override bool TryExecuteWorker(IncidentParms parms)
    //    {
    //        Log.Message(
    //            "We made it to new incidentworker_raidenemy yay!.  Number of raids is" +
    //            Find.StoryWatcher.statsRecord.numRaidsEnemy, true);
    //        base.TryExecuteWorker(parms);

    //        if (Find.StoryWatcher.statsRecord.numRaidsEnemy != 1)
    //        {
    //            return true;
    //        }

    //        Log.Message("Here we are.  Number of raids is " + Find.StoryWatcher.statsRecord.numRaidsEnemy);
    //        var map = (Map) parms.target;
    //        if (!RCellFinder.TryFindRandomPawnEntryCell(out var result, map, CellFinder.EdgeRoadChance_Animal))
    //        {
    //            return false;
    //        }

    //        foreach (var p in PawnsFinder.AllMaps_SpawnedPawnsInFaction(Faction.OfPlayer))
    //        {
    //            if (p.kindDef == DefDatabase<PawnKindDef>.GetNamed("HenThirteen"))
    //            {
    //                return false;
    //            }
    //        }

    //        var loc = CellFinder.RandomClosewalkCellNear(result, map, 12);
    //        var kind = DefDatabase<PawnKindDef>.GetNamed("HenThirteen");
    //        var pawn = PawnGenerator.GeneratePawn(kind);
    //        GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
    //        //PawnBioAndNameGenerator.GeneratePawnName(pawn, NameStyle.Full, "Hen 13");
    //        pawn.Name = new NameTriple("Hen", "Hero Chicken", "13");
    //        pawn.SetFaction(Faction.OfPlayer);
    //        Find.LetterStack.ReceiveLetter("Hen 13 Joins",
    //            "A mythical hero of wide renown has sensed your danger and has decided to join your cause.  Hen Thirteen is on the scene",
    //            LetterDefOf.PositiveEvent, new TargetInfo(result, map));

    //        return true;
    //    }
    //}
}