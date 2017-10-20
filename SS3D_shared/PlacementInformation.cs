﻿using System;

namespace SS13_Shared
{
    public class PlacementInformation
    {
        public string EntityType; //Object name / type. If not tile.
        public Boolean IsTile;
        public int MobUid; //UID of mob this permission is for.
        public string PlacementOption;
        public int Range; //Valid range from mob.

        public string TileType = ""; //Tile Type if tile.
        public int Uses = 1; //How many objects of this type may be placed.
    }
}