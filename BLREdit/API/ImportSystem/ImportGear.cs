using System.Collections.Generic;

namespace BLREdit
{
    public class ImportGear : BLREdit.API.ImportSystem.IImportCategory
    {
        public List<ImportItem> attachments { get; set; }
        public List<ImportItem> avatars { get; set; }
        public List<ImportItem> badges { get; set; }
        public object[] crosshairs { get; set; }
        public List<ImportItem> emotes { get; set; }
        public List<ImportItem> hangers { get; set; }
        public List<ImportItem> helmets { get; set; }
        public List<ImportItem> lowerBodies { get; set; }
        public List<ImportItem> tactical { get; set; }
        public List<ImportItem> upperBodies { get; set; }

        internal void AssignWikiStats(WikiStats[] stats)
        {
            ImportSystem.AssignWikiStatsTo(attachments, stats);
            ImportSystem.AssignWikiStatsTo(avatars, stats);
            ImportSystem.AssignWikiStatsTo(badges, stats);
            ImportSystem.AssignWikiStatsTo(emotes, stats);
            ImportSystem.AssignWikiStatsTo(hangers, stats);
            ImportSystem.AssignWikiStatsTo(helmets, stats);
            ImportSystem.AssignWikiStatsTo(lowerBodies, stats);
            ImportSystem.AssignWikiStatsTo(tactical, stats);
            ImportSystem.AssignWikiStatsTo(upperBodies, stats);
        }

        public WikiStats[] GetWikiStats()
        {
            List<WikiStats> stats = new();
            stats.AddRange(ImportSystem.GetWikiStats(attachments));
            stats.AddRange(ImportSystem.GetWikiStats(avatars));
            stats.AddRange(ImportSystem.GetWikiStats(badges));
            stats.AddRange(ImportSystem.GetWikiStats(emotes));
            stats.AddRange(ImportSystem.GetWikiStats(hangers));
            stats.AddRange(ImportSystem.GetWikiStats(helmets));
            stats.AddRange(ImportSystem.GetWikiStats(lowerBodies));
            stats.AddRange(ImportSystem.GetWikiStats(tactical));
            stats.AddRange(ImportSystem.GetWikiStats(upperBodies));
            return stats.ToArray();
        }

        public void CleanItems()
        {
            ImportSystem.CleanItems(attachments, "attachments");
            ImportSystem.CleanItems(avatars, "avatar");
            ImportSystem.CleanItems(badges, "badge");
            ImportSystem.CleanItems(emotes, "emote");
            ImportSystem.CleanItems(hangers, "hanger");
            ImportSystem.CleanItems(helmets, "helmet");
            ImportSystem.CleanItems(lowerBodies, "lowerBody");
            ImportSystem.CleanItems(tactical, "tactical");
            ImportSystem.CleanItems(upperBodies, "upperBody");
        }

        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}
