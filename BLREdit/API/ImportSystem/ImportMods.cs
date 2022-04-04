using System.Collections.Generic;

namespace BLREdit
{
    public class ImportMods
    {
        public ImportItem[] ammo { get; set; }
        public ImportItem[] ammos { get; set; }
        public ImportItem[] barrels { get; set; }
        public ImportItem[] camosBody { get; set; }
        //public ImportItem[] camosWeapon { get; set; }
        public ImportItem[] grips { get; set; }
        public ImportItem[] magazines { get; set; }
        public ImportItem[] muzzles { get; set; }
        public ImportItem[] primarySkins { get; set; }
        public ImportItem[] scopes { get; set; }
        public object[] secondarySkins { get; set; }
        public ImportItem[] stocks { get; set; }

        internal void AssignWikiStats(WikiStats[] stats)
        {
            ImportSystem.AssignWikiStatsTo(barrels, stats);
            ImportSystem.AssignWikiStatsTo(magazines, stats);
            ImportSystem.AssignWikiStatsTo(muzzles, stats);
            ImportSystem.AssignWikiStatsTo(scopes, stats);
            ImportSystem.AssignWikiStatsTo(stocks, stats);
        }

        public WikiStats[] GetWikiStats()
        {
            List<WikiStats> stats = new();
            stats.AddRange(ImportSystem.GetWikiStats(barrels));
            stats.AddRange(ImportSystem.GetWikiStats(magazines));
            stats.AddRange(ImportSystem.GetWikiStats(muzzles));
            stats.AddRange(ImportSystem.GetWikiStats(scopes));
            stats.AddRange(ImportSystem.GetWikiStats(stocks));
            return stats.ToArray();
        }

        public ImportMods() { }
        public ImportMods(ImportMods mods)
        {
            ammo = ImportSystem.CleanItems(mods.ammo, "ammo");
            ammos = ImportSystem.CleanItems(mods.ammos, "ammos");
            barrels = ImportSystem.CleanItems(mods.barrels, "barrel");
            camosBody = ImportSystem.CleanItems(mods.camosBody, "camoBody");
            //camosWeapon = ImportSystem.CleanItems(mods.camosWeapon, "camoWeapon");
            grips = ImportSystem.CleanItems(mods.grips, "grip");
            magazines = ImportSystem.CleanItems(mods.magazines, "magazine");
            muzzles = ImportSystem.CleanItems(mods.muzzles, "muzzle");
            primarySkins = ImportSystem.CleanItems(mods.primarySkins, "primarySkin");
            scopes = ImportSystem.CleanItems(mods.scopes, "scope");
            stocks = ImportSystem.CleanItems(mods.stocks, "stock");
        }
        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}
