using System;

public enum StatusEffectTypes
{
    Damage,
    Heal,
    Defense,
    Sleep
}

[Serializable]
public class StatusEffect
{
    public StatusEffectTypes type;
    public int turnDuration;
    public int turnsRemaining;
    public int amount;
    public string phrase;
    public string moveName;

    public StatusEffect Copy(string moveName = "The move")
    {
        return new StatusEffect
        {
            type = this.type,
            turnDuration = this.turnDuration,
            turnsRemaining = this.turnDuration, // putting this here on purpose to initially set turnsRemaining
            amount = this.amount,
            phrase = this.phrase,
            moveName = moveName
        };
    }
}
