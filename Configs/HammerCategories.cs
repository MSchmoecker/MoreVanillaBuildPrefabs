﻿// Ignore Spelling: MVBP

using BepInEx.Configuration;
using MVBP.Extensions;

namespace MVBP.Configs
{
    /// <summary>
    ///     Helper class to get the names of hammer piece categories for this mod.
    /// </summary>
    internal static class HammerCategories
    {
        public const string CreatorShop = "CreatorShop";
        public const string Nature = "Nature";
        public const string Misc = "Misc";
        public const string Crafting = "Crafting";
        public const string Building = "BuildingWorkbench";
        public const string Stonecutter = "BuildingStonecutter";
        public const string Furniture = "Furniture";

        internal static AcceptableValueList<string> GetAcceptableValueList()
        {
            return new AcceptableValueList<string>(typeof(HammerCategories).GetAllPublicConstantValues<string>().ToArray());
        }
    }
}