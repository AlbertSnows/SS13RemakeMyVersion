﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SS13.Graphics
{
    [Serializable]
    public class AnimationCollection
    {
        public string Name { get; set; }
        public List<AnimationInfo> Animations { get; set; } 

        public AnimationCollection()
        {
            Animations = new List<AnimationInfo>();
        }
    }
}
