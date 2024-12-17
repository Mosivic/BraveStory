﻿using System;
using System.Collections.Generic;

namespace Miros.Core;

public class CompoundState : State
{
    public virtual State[] SubStates { get; set; }
}