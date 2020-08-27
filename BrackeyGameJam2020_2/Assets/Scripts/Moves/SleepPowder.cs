using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepPowder : Move
{
    public StatusEffect effect;

    private BattleDataModel _model;

    private SpriteRenderer _renderer;

    public override IEnumerator Execute(BattleDataModel model)
    {
        Init();

        _model = model;

        transform.position = model.targetUnit.transform.position;

        StartCoroutine(Damage());

        yield return null;
    }

    // Start is called before the first frame update
    private void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
    }

    private IEnumerator Damage()
    {
        yield return new WaitForSeconds(0.6f);

        _renderer.enabled = false;

        _model.targetUnit.SetSkipTurn(true);

        _model.targetUnit.AddEffect(effect.Copy(GetName()));
        _model.dialogueText.text = $"Sleep Powder caused {_model.targetUnit.unitName} to fall asleep!";

        yield return new WaitForSeconds(2f);

        _model.battleSystem.SetState(_model.nextState);

        Destroy(gameObject);
    }

    public override string GetName()
    {
        return "Sleep Powder";
    }
}
