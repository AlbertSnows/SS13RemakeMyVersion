﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SS13_Shared;

namespace ServerInterfaces.GOC
{
    public interface IWallMountedComponent
    {
        void AttachToTile(Vector2 tilePos);
    }
}
