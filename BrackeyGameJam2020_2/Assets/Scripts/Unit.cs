using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;
    public int experience;
    public int experienceToNextLevel = 100;
    public int damage;
    public int maxHP;
    public int currentHP;
    public int defense;

    public bool isDead;

    public bool skipTurns;

    public List<string> enemyMoves;

    public string offensiveMove1Name;
    public string offensiveMove2Name;
    public string defensiveMoveName;

    public List<StatusEffect> activeEffects;

    public Transform backTarget;
    public Transform launcher;

    public BattleHUD hud;

    public BattleState finishState;

    public Text dialogueText;

    private float _distanceToBackTarget;

    private BattleSystem battleSystem;

    void Start()
    {
        activeEffects = new List<StatusEffect>();
        _distanceToBackTarget = Math.Abs(transform.position.x - backTarget.position.x);
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    public IEnumerator OnTurn(Action callback = null)
    {
        foreach(var effect in activeEffects.ToArray())
        {
            if (effect.turnsRemaining <= 0)
            {
                switch(effect.type)
                {
                    case StatusEffectTypes.Defense:
                        defense -= effect.amount;
                        dialogueText.text = string.Format(effect.phrase, unitName, effect.moveName);
                        yield return new WaitForSeconds(0.6f);
                        break;
                    case StatusEffectTypes.Sleep:
                        SetSkipTurn(false);
                        dialogueText.text = string.Format(effect.phrase, unitName, effect.moveName);
                        yield return new WaitForSeconds(0.6f);
                        break;
                }

                activeEffects.Remove(effect);
                continue;
            }

            dialogueText.text = "";
            yield return new WaitForSeconds(0.4f);

            switch(effect.type)
            {
                case StatusEffectTypes.Damage:
                    isDead = TakeDamage(effect.amount, true).Item2;

                    dialogueText.text = string.Format(effect.phrase, unitName, effect.moveName);
                    hud.SetHP(currentHP);

                    yield return new WaitForSeconds(1.5f);

                    if (isDead)
                    {
                        battleSystem.SetState(finishState);
                    }
                    break;
                case StatusEffectTypes.Heal:
                    break;
            }

            effect.turnsRemaining--;
        }

        if (!isDead)
        {
            callback?.Invoke();
        }
    }

    public void SetSkipTurn(bool skipTurns)
    {
        this.skipTurns = skipTurns;
    }

    public bool SkipTurn()
    {
        return skipTurns;
    }

    public void Heal(int amount)
    {
        currentHP = Math.Min(currentHP + amount, maxHP);
    }

    public Tuple<bool, bool> TakeDamage(int damage, bool ignoreDefense = false)
    {
        if (!ignoreDefense && defense > damage)
        {
            StartCoroutine(BlockedDamage());
            return new Tuple<bool, bool>(false, false);
        }

        currentHP -= damage;

        return new Tuple<bool, bool>(true, currentHP <= 0);
    }

    private IEnumerator BlockedDamage()
    {
        dialogueText.text = $"{unitName} blocked the attack!";
        yield return new WaitForSeconds(1f);
    }

    public bool LevelUp(int xp)
    {
        experience += xp;

        if (experience >= experienceToNextLevel)
        {
            experienceToNextLevel += (unitLevel * 25) + 100;
            unitLevel++;
            maxHP += 3;
            damage += 1;
            currentHP = maxHP;
            return true;
        }

        return false;
    }

    public void AddEffect(StatusEffect effect)
    {
        activeEffects.Add(effect);
    }

    void Update()
    {
        var point = Camera.main.WorldToScreenPoint(backTarget.position);

        if (point.x < 0)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(transform.position).x + Math.Abs(point.x) + _distanceToBackTarget, 0, 0));
            transform.position = new Vector3(pos.x, transform.position.y, 0);
        } else if (point.x > Screen.width)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.WorldToScreenPoint(transform.position).x - Math.Abs(point.x - Screen.width) - _distanceToBackTarget, 0, 0));
            transform.position = new Vector3(pos.x, transform.position.y, 0);
        }
    }
}
