using System.Collections.Generic;

namespace BLREdit
{
    public class ImportWeapons : BLREdit.API.ImportSystem.IImportCategory
    {
        public List<ImportItem> depot { get; set; }
        public List<ImportItem> primary { get; set; }
        public List<ImportItem> secondary { get; set; }

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

        public void CleanItems()
        {
            ImportSystem.CleanItems(depot, "depot");
            ImportSystem.CleanItems(primary, "primary");
            ImportSystem.CleanItems(secondary, "secondary");
        }

        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}
