
using System.Transactions;
using Godot;
using GPC.Job.Config;

class PlayerState : State{
    public CharacterBody2D Host {get;set;}
    public AnimationPlayer AnimationPlayer{get;set;} 
    public Sprite2D Sprite {get;set;}

    public float Gravity {get;} = 980;
	public float RunSpeed{get;} = 200;
	public float JumpVeocity{get;} = -300;

    public PlayerState(){
        AnimationPlayer = Host.GetNode<AnimationPlayer>("AnimationPlayer");
        Sprite = Host.GetNode<Sprite2D>("Sprite"); 
    }

}