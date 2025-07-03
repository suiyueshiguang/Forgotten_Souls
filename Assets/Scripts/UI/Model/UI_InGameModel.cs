public enum SkillInGameType
{
    Dash,
    Parry,
    Crystal,
    Sword,
    BlackHole,
    Flask
}

public class UI_InGameModel
{
    public float hpValue;
    public float hpMaxValue;
    public float hpEffectValue;
    public float hpEffectMaxValue;
    public float hpEffectSpeed;

    public float dashCooldown;
    public float parryCooldown;
    public float crystalCooldown;
    public float swordCooldown;
    public float blackHoleCooldown;
    public float flaskCooldown;

    public int currentSouls;
    public float increaseRate;
}