using System;

namespace Miros.Core;

[Flags]
public enum SupportedOperation
{
    None = 0,
    Add = 1 << 0,
    Multiply = 1 << 1,
    Divide = 1 << 2,
    Override = 1 << 3,
    All = Add | Multiply | Divide | Override
}
