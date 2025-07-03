using System;
using RimWorld;
using Verse;

namespace HenThirteen;

public class HenThirteen : Pawn
{
    private ThingDef_HenThirteen Def =>
        //Case-sensitive! If you use Def it will return Def, which is this getter. This will cause a never ending cycle and a stack overflow.
        def as ThingDef_HenThirteen;

    public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        base.PreApplyDamage(ref dinfo, out absorbed);

        if (Faction != Faction.OfPlayer)
        {
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

        var percentHealth = Math.Min(DamageAbsorbed / (HealthScale * 25.0f), 0.99f);
        var rand = Rand.Value;
        absorbed = rand <= percentHealth * Def.ZerkFactor;

        if (absorbed)
        {
            Messages.Message(
                "HeTh.absorb".Translate(dinfo.Instigator.LabelShort, dinfo.Weapon.label),
                MessageTypeDefOf.PositiveEvent);
        }
    }
}