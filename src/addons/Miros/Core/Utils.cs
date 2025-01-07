
using System;

namespace Miros.Core;

public class Utils{
    public static float ApplyOperation(ModifierOperation operation, float oldValue, float newValue)
    {
        switch (operation)
        {
            case ModifierOperation.Add:
                return oldValue + newValue;
            case ModifierOperation.Minus:
                return oldValue - newValue;
            case ModifierOperation.Multiply:
                return oldValue * newValue;
            case ModifierOperation.Divide:
                return oldValue / newValue;
            case ModifierOperation.Override:
                return newValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}