using System.Collections.Generic;

namespace BLREdit
{
    public class ImportMods : BLREdit.API.ImportSystem.IImportCategory
    {
        public List<ImportItem> ammo { get; set; }
        public List<ImportItem> ammos { get; set; }
        public List<ImportItem> barrels { get; set; }
        public List<ImportItem> camosBody { get; set; }
        //public List<ImportItem> camosWeapon { get; set; }
        public List<ImportItem> grips { get; set; }
        public List<ImportItem> magazines { get; set; }
        public List<ImportItem> muzzles { get; set; }
        public List<ImportItem> primarySkins { get; set; }
        public List<ImportItem> scopes { get; set; }
        public object[] secondarySkins { get; set; }
        public List<ImportItem> stocks { get; set; }

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

        public void CleanItems()
        {
            ImportSystem.CleanItems(ammo, "ammo");
            ImportSystem.CleanItems(ammos, "ammos");
            ImportSystem.CleanItems(barrels, "barrel");
            ImportSystem.CleanItems(camosBody, "camoBody");
            //camosWeapon = ImportSystem.CleanItems(mods.camosWeapon, "camoWeapon");
            ImportSystem.CleanItems(grips, "grip");
            ImportSystem.CleanItems(magazines, "magazine");
            ImportSystem.CleanItems(muzzles, "muzzle");
            ImportSystem.CleanItems(primarySkins, "primarySkin");
            ImportSystem.CleanItems(scopes, "scope");
            ImportSystem.CleanItems(stocks, "stock");
        }
        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}
