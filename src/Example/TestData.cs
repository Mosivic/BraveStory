using Godot;

namespace BraveStory;

public class TestData 
{
public class MagicData
{
    public bool elementType = true;
    public int magicDamage = 5;
}

public class SwordData
{
    public float attackSpeed = 1.2f;
    public string weaponType = "melee";
}

public class WeaponData
{
    public int baseDamage = 10;
    public int durabilityMax = 100;
}

public class ItemData
{
    public bool stackable = false;
    public float weight = 1.0f;
}

public class BuffData
{
    public int duration = 10;
}

public class DebuffData
{
    public int duration = 5;
}

public class BaseData
{
    public float AirAcceleration = 800.0f;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
}

public class WarriorData
{
    public int Defense = 8;
    public int Strength = 10;
}

public class PlayerData
{
    public float AirAcceleration = 800.0f;
    public float attackSpeed = 1.2f;
    public int Defense = 8;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public int Strength = 10;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
    public string weaponType = "melee";
}

public class EnemyData
{
    public float AirAcceleration = 800.0f;
    public float FloorAcceleration = 1000.0f;
    public float Gravity = 980.0f;
    public float JumpVelocity = -300.0f;
    public float RunSpeed = 200.0f;
    public float WalkSpeed = 80.0f;
    public Vector2 WallJumpVelocity = new(1000f, -320f);
}
}