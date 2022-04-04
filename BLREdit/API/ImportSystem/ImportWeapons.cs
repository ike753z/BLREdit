using System.Collections.Generic;

namespace BLREdit
{
    public class ImportWeapons
    {
        public ImportItem[] depot { get; set; }
        public ImportItem[] primary { get; set; }
        public ImportItem[] secondary { get; set; }

        internal void AssignWikiStats(WikiStats[] stats)
        {
            ImportSystem.AssignWikiStatsTo(depot, stats);
            ImportSystem.AssignWikiStatsTo(primary, stats);
            ImportSystem.AssignWikiStatsTo(secondary, stats);
        }

        public WikiStats[] GetWikiStats()
        {
            List<WikiStats> stats = new();
            stats.AddRange(ImportSystem.GetWikiStats(depot));
            stats.AddRange(ImportSystem.GetWikiStats(primary));
            stats.AddRange(ImportSystem.GetWikiStats(secondary));
            return stats.ToArray();
        }

        public ImportWeapons() { }
        public ImportWeapons(ImportWeapons weapons)
        {
            depot = ImportSystem.CleanItems(weapons.depot, "depot");
            primary = ImportSystem.CleanItems(weapons.primary, "primary");
            secondary = ImportSystem.CleanItems(weapons.secondary, "secondary");
        }

        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}
