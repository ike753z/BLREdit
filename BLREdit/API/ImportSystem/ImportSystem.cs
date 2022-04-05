using BLREdit.API.Utils;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLREdit
{

    public static class ImportSystem
    {
        private static bool AddFolderLine = AppDomain.CurrentDomain.BaseDirectory.EndsWith("\\");

        public static readonly Dictionary<float, float> DamagePercentToValue = new();

        //public static readonly FoxIcon[] Icons = LoadAllIcons();
        //public static readonly FoxIcon[] Crosshairs = LoadAllCrosshairs();
        //private static readonly ImportGear importGear 
        //private static readonly ImportMods importMods 
        //private static readonly ImportWeapons importWeapons 

        private static WikiStats[] CorrectedItemStats;

        private static IniStats[] IniItemStats;

        public static ImportGear Gear { get; private set; }
        public static ImportMods Mods { get; private set; }
        public static ImportWeapons Weapons { get; private set; }

        public static void Initialize()
        {
            var watch = LoggingSystem.Log("Initializing Import System", LogTypes.Timing);

            LoadItems();
            CleanItems();
            LoadWikiStats();
            LoadIniStats();

            //UpgradeIniStats();

            //LoggingSystem.Log("BodyCamoCount:" + Mods.camosBody.Length);
            //LoggingSystem.LogInfo("WeaponCamoCount:" + Mods.camosWeapon.Length);

            LoggingSystem.Append(watch, "Import System");
        }

        private static void LoadItems()
        {
            var watch = LoggingSystem.Log("Loading Item Lists", LogTypes.Timing, "");
            Gear = IOResources.DeserializeFile<ImportGear>(IOResources.GEAR_FILE);
            Mods = IOResources.DeserializeFile<ImportMods>(IOResources.MOD_FILE);
            Weapons = IOResources.DeserializeFile<ImportWeapons>(IOResources.WEAPON_FILE);
            LoggingSystem.Append(watch, "");
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static void UpgradeIniStats()
#pragma warning restore IDE0051 // Remove unused private members
        {
            IOResources.SerializeFile<IniStats[]>("upgraded.json", IniItemStats);
        }

        private static void CleanItems()
        {
            var watch = LoggingSystem.Log("Cleaning Items", LogTypes.Timing, "");
            Gear.CleanItems();
            Mods.CleanItems();
            Weapons.CleanItems();
            LoggingSystem.Append(watch);
        }

        public static void CleanItems(List<ImportItem> importItems, string categoryName)
        {
            List<ImportItem> toRemove = new();
            foreach (ImportItem item in importItems)
            {
                if (IsValidItem(item))
                {
                    item.Category = categoryName;

                    if (!AddFolderLine)
                    {
                        item.MaleWide = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "wide\\" + item.icon + ".png", UriKind.Absolute);
                        item.MaleSmall = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "squareSmall\\" + item.icon + ".png", UriKind.Absolute);

                        item.FemaleWide = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "genderWide\\" + item.icon + ".png", UriKind.Absolute);
                        item.FemaleSmall = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "genderSmall\\" + item.icon + ".png", UriKind.Absolute);
                        if (categoryName == "scope")
                        {
                            item.Scope = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "previewLarge\\" + item.name + ".png", UriKind.Absolute);
                            item.MiniScope = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + IOResources.IMG_CACHE + "previewSmall\\" + item.name + ".png", UriKind.Absolute);
                        }
                        else
                        {
                            item.Scope = null;
                            item.MiniScope = null;
                        }
                    }
                    else
                    {
                        item.MaleWide = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "wide\\" + item.icon + ".png", UriKind.Absolute);
                        item.MaleSmall = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "squareSmall\\" + item.icon + ".png", UriKind.Absolute);

                        item.FemaleWide = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "genderWide\\" + item.icon + ".png", UriKind.Absolute);
                        item.FemaleSmall = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "genderSmall\\" + item.icon + ".png", UriKind.Absolute);
                        if (categoryName == "scope")
                        {
                            item.Scope = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "previewLarge\\" + item.name + ".png", UriKind.Absolute);
                            item.MiniScope = new Uri(AppDomain.CurrentDomain.BaseDirectory + IOResources.IMG_CACHE + "previewSmall\\" + item.name + ".png", UriKind.Absolute);
                        }
                        else
                        {
                            item.Scope = null;
                            item.MiniScope = null;
                        }
                    }
                }
                else
                { 
                    toRemove.Add(item);
                }
            }
            foreach (ImportItem item in toRemove)
            { 
                importItems.Remove(item);
            }
        }

        private static void LoadWikiStats()
        {
            CorrectedItemStats = LoadWikiStatsFromCSV();
            AssignWikiStats(CorrectedItemStats);
            GenerateWikiStats();
        }

        private static void LoadIniStats()
        {
            IniItemStats = IOResources.DeserializeFile<IniStats[]>(IOResources.ASSET_DIR + "\\filteredIniStats.json");

            AssignIniStatsTo(Weapons.primary, IniItemStats);
            AssignIniStatsTo(Weapons.secondary, IniItemStats);
        }

        public static IniStats[] GetFromWeapons(List<ImportItem> items1, List<ImportItem> items2)
        {
            List<IniStats> stats = new();
            foreach (ImportItem item in items1)
            {
                if (item.IniStats != null)
                {
                    stats.Add(item.IniStats);
                }
            }
            foreach (ImportItem item in items2)
            {
                if (item.IniStats != null)
                {
                    stats.Add(item.IniStats);
                }
            }
            return stats.ToArray();
        }

        public static ImportItem GetItemByID(int index, List<ImportItem> items)
        {
            if (index < 0)
            { return null; }
            return items[index];
        }


        public static ImportItem GetItemByName(string name, List<ImportItem> items)
        {
            foreach (ImportItem item in items)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        internal static void AssignIniStatsTo(List<ImportItem> items, IniStats[] stats)
        {
            foreach (ImportItem item in items)
            {
                bool found = false;
                foreach (IniStats stat in stats)
                {
                    if (stat.ItemID == item.uid)
                    {
                        item.IniStats = stat;
                        found = true;
                    }
                }
                if (!found)
                {
                    LoggingSystem.Log("No IniStats for " + item.name);
                }
            }
        }

        public static int GetGearID(ImportItem item)
        {
            return GetItemID(item, Gear.attachments);
        }
        public static int GetTacticalID(ImportItem item)
        {
            return GetItemID(item, Gear.tactical);
        }

        public static int GetMuzzleID(ImportItem item)
        {
            return GetItemID(item, Mods.muzzles);
        }
        public static int GetMagazineID(ImportItem item)
        {
            return GetItemID(item, Mods.magazines);
        }
        public static int GetTagID(ImportItem item)
        {
            return GetItemID(item, Gear.hangers);
        }
        public static int GetHelmetID(ImportItem item)
        {
            return GetItemID(item, Gear.helmets);
        }
        public static int GetUpperBodyID(ImportItem item)
        {
            return GetItemID(item, Gear.upperBodies);
        }
        public static int GetLowerBodyID(ImportItem item)
        {
            return GetItemID(item, Gear.lowerBodies);
        }
        public static int GetCamoBodyID(ImportItem item)
        {
            return GetItemID(item, Mods.camosBody);
        }
        public static int GetItemID(ImportItem item, List<ImportItem> items)
        {
            if (item == null) { return -1; }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].uid == item.uid)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static void AssignWikiStatsTo(List<ImportItem> items, WikiStats[] stats)
        {
            foreach (ImportItem item in items)
            {
                bool found = false;
                foreach (WikiStats stat in stats)
                {
                    if (!found && stat.itemID == item.uid)
                    {
                        item.WikiStats = new WikiStats()
                        {
                            itemID = item.uid,
                            itemName = item.name,
                            aimSpread = stat.aimSpread,
                            ammoMag = stat.ammoMag,
                            ammoReserve = stat.ammoReserve,
                            damage = stat.damage,
                            firerate = stat.firerate,
                            hipSpread = stat.hipSpread,
                            moveSpread = stat.moveSpread,
                            rangeClose = stat.rangeClose,
                            rangeFar = stat.rangeFar,
                            recoil = stat.recoil,
                            reload = stat.reload,
                            run = stat.run,
                            scopeInTime = stat.scopeInTime,
                            swaprate = stat.swaprate,
                            zoom = stat.zoom
                        };
                        found = true;
                    }
                }
            }
        }

        internal static void AssignWikiStats(WikiStats[] stats)
        {
            Gear.AssignWikiStats(stats);
            Mods.AssignWikiStats(stats);
            Weapons.AssignWikiStats(stats);
        }

        internal static void GenerateWikiStats()
        {
            List<WikiStats> stats = new();

            stats.AddRange(Gear.GetWikiStats());
            stats.AddRange(Mods.GetWikiStats());
            stats.AddRange(Weapons.GetWikiStats());
        }

        internal static WikiStats[] GetWikiStats(List<ImportItem> items)
        {
            List<WikiStats> stats = new();
            foreach (var item in items)
            {
                stats.Add(item.WikiStats);
            }
            return stats.ToArray();
        }

        internal static WikiStats[] LoadWikiStatsFromCSV()
        {
            List<WikiStats> stats = new();
            StreamReader sr = new(IOResources.ASSET_DIR + "\\BLR Wiki Stats.csv");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                stats.Add(new WikiStats()
                {
                    itemName = parts[0],
                    itemID = int.Parse(parts[1]),
                    damage = float.Parse(parts[2], CultureInfo.InvariantCulture),
                    firerate = float.Parse(parts[3], CultureInfo.InvariantCulture),
                    ammoMag = float.Parse(parts[4], CultureInfo.InvariantCulture),
                    ammoReserve = float.Parse(parts[5], CultureInfo.InvariantCulture),
                    reload = float.Parse(parts[6], CultureInfo.InvariantCulture),
                    swaprate = float.Parse(parts[7], CultureInfo.InvariantCulture),
                    aimSpread = float.Parse(parts[8], CultureInfo.InvariantCulture),
                    hipSpread = float.Parse(parts[9], CultureInfo.InvariantCulture),
                    moveSpread = float.Parse(parts[10], CultureInfo.InvariantCulture),
                    recoil = float.Parse(parts[11], CultureInfo.InvariantCulture),
                    zoom = float.Parse(parts[12], CultureInfo.InvariantCulture),
                    scopeInTime = float.Parse(parts[13], CultureInfo.InvariantCulture),
                    rangeClose = float.Parse(parts[14], CultureInfo.InvariantCulture),
                    rangeFar = float.Parse(parts[15], CultureInfo.InvariantCulture),
                    run = float.Parse(parts[16], CultureInfo.InvariantCulture)
                });
            }
            sr.Close();
            return stats.ToArray();
        }

        internal static void CreateImageCache()
        {
            string cachedJsonFile = "Cache\\cached.json";
            var timer = LoggingSystem.Log("Image Cache Creation: ", LogTypes.Timing, "");

            List<string> AlreadyCached = new();

            if (Directory.Exists("Cache\\") && File.Exists(cachedJsonFile)) 
            {
                // TODO: Get Cached ItemImage List
                AlreadyCached = IOResources.DeserializeFile<List<string>>(cachedJsonFile);
                //File.Delete(cachedJsonFile); Dont have to delete it IO Resources will do that for us when serializing it again
            }

            Directory.CreateDirectory("Cache\\wide\\");
            Directory.CreateDirectory("Cache\\genderWide\\");
            Directory.CreateDirectory("Cache\\genderSmall\\");
            //Directory.CreateDirectory("Cache\\squareLarge\\");
            Directory.CreateDirectory("Cache\\squareSmall\\");
            Directory.CreateDirectory("Cache\\previewLarge\\");
            Directory.CreateDirectory("Cache\\previewSmall\\");

            List<ImportItem> allUsedItems = new();
            allUsedItems.AddRange(Weapons.primary);
            allUsedItems.AddRange(Weapons.secondary);
            allUsedItems.AddRange(Mods.muzzles);
            allUsedItems.AddRange(Mods.barrels);
            allUsedItems.AddRange(Mods.grips);
            allUsedItems.AddRange(Mods.magazines);
            allUsedItems.AddRange(Mods.scopes);
            allUsedItems.AddRange(Mods.stocks);
            allUsedItems.AddRange(Mods.camosBody);
            allUsedItems.AddRange(Gear.helmets);
            allUsedItems.AddRange(Gear.upperBodies);
            allUsedItems.AddRange(Gear.lowerBodies);
            allUsedItems.AddRange(Gear.hangers);
            allUsedItems.AddRange(Gear.attachments);
            allUsedItems.AddRange(Gear.tactical);

            var deduplicated = allUsedItems.GroupBy(x => x.icon).Select(x => x.First()).ToList();
            foreach (var item in AlreadyCached)
            {
                for(int i = 0; i < deduplicated.Count; i++)
                {
                    if (deduplicated[i].name.Equals(item))
                    { 
                        deduplicated.RemoveAt(i);
                        continue;
                    }
                }
            }

            BLREdit.UI.ProgressBar bar = new UI.ProgressBar(ref deduplicated, ref AlreadyCached);
            bar.ShowDialog();

            IOResources.SerializeFile(cachedJsonFile, AlreadyCached, true, false);

            LoggingSystem.Append(timer);
        }


        public static bool IsValidItem(ImportItem item)
        {
            return item.tooltip != "SHOULDN'T BE USED" && !string.IsNullOrEmpty(item.name);
        }
    }
}