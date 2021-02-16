/********************
 * Explorite模组信息类。
 * --siiftun1857
 */
using UnityEngine;
using Verse;

namespace Explorite
{
    ///<summary>Explorite模组信息类。</summary>
    public class ExploriteMod : Mod
    {
        public static ExploriteSettings settings;

        public override string SettingsCategory() => "Alien Race";

        public ExploriteMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<ExploriteSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            //listingStandard.CheckboxLabeled(label: "something", ref settings.something, tooltip: "True: something\nFalse: something");
            listingStandard.End();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            settings.UpdateSettings();
        }
    }

    public class ExploriteSettings : ModSettings
    {
        public bool something;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref something, label: "something", defaultValue: false);
        }

        public void UpdateSettings() { }
    }
}
