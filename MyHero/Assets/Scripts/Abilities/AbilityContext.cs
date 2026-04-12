using UnityEngine;

public class AbilityContext
{
    public CharacterStats caster;
    public CharacterStats target;
    public Vector3 point;

    public AbilityContext(CharacterStats caster, CharacterStats target = null, Vector3 point = default)
    {
        this.caster = caster;
        this.target = target;
        this.point = point;
    }
}
