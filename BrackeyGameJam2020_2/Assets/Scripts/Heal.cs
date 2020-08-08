using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Move
{
    public int healAmount;

    private BattleDataModel _model;

    private SpriteRenderer _renderer;

    public override IEnumerator Execute(BattleDataModel model)
    {
        Init();

        _model = model;

        transform.position = model.sourceUnit.transform.position;

        StartCoroutine(HealSelf());

        yield return null;
    }

    // Start is called before the first frame update
    private void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
    }

    private IEnumerator HealSelf()
    {
        yield return new WaitForSeconds(0.6f);

        _model.sourceUnit.Heal(healAmount);

        _renderer.enabled = false;

        _model.dialogueText.text = $"{_model.sourceUnit.unitName}'s healed themself!";

        yield return new WaitForSeconds(2f);

        _model.battleSystem.SetState(_model.nextState);

        Destroy(gameObject);
    }

    public override string GetName()
    {
        return "Heal";
    }
}
