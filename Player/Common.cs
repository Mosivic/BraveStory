﻿using Godot;
using GPC.Evaluator;

namespace BraveStory.Player;

public class Common
{
    
}

public class Evaluators
{
    public static readonly Evaluator<bool> IsJumpKeyDown = 
        new(() => Input.IsActionJustPressed("jump"));


    public static readonly Evaluator<bool> IsMoveKeyDown =
        new(() => !Mathf.IsZeroApprox(Input.GetAxis("move_left", "move_right")));

}
