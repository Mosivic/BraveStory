using System;
using System.Collections.Generic;

namespace Miros.Core;

public class GrabberAggregator
{
    private readonly Dictionary<AttributeBase, Grabber> _grabberCache = [];
}0