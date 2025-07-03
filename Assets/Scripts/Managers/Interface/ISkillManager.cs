using UnityEngine;

public interface ISkillManager
{
    public Dash_Skill GetDash();
    public Clone_Skill GetClone();
    public Sword_Skill GetSword();
    public Blackhole_Skill GetBlackhole();
    public Crystal_Skill GetCrystal();
    public Parry_Skill GetParry();
    public Dodge_Skill GetDodge();
}
