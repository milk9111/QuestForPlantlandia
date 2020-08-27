using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushGrow : Move
{
    public StatusEffect effect;

    private BattleDataModel _model;

    private SpriteRenderer _renderer;

    public override IEnumerator Execute(BattleDataModel model)
    {
        Init();

        _model = model;

        transform.position = model.sourceUnit.transform.position;

        StartCoroutine(RaiseDefense());

        yield return null;
    }

    // Start is called before the first frame update
    private void Init()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
    }

    private IEnumerator RaiseDefense()
    {
        yield return new WaitForSeconds(0.6f);

        _model.sourceUnit.defense += effect.amount;

        _renderer.enabled = false;

        _model.sourceUnit.AddEffect(effect.Copy(GetName()));
        _model.dialogueText.text = $"Bush Grow increased {_model.sourceUnit.unitName}'s defense!";

        yield return new WaitForSeconds(2f);

        _model.battleSystem.SetState(_model.nextState);

        Destroy(gameObject);
    }

    public override string GetName()
    {
        return "Bush Grow";
    }
}
