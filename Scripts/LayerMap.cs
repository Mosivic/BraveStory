﻿using System.Collections.Generic;
using GPC;

namespace BraveStory.Scripts;

public static class LayerMap
{
    
    public static readonly Layer Movement = new("Movement", null);
    public static readonly Layer Buff = new("Buff", null);
    
    public static readonly Layer Root = new("Root", [Movement, Buff]);
}