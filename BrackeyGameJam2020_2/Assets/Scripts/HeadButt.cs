using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadButt : Move
{
    public int damage;

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

        var result = _model.targetUnit.TakeDamage(_model.sourceUnit.damage + damage);
        _renderer.enabled = false;

        if (result.Item1)
        {
            _model.targetHUD.SetHP(_model.targetUnit.currentHP);
            _model.dialogueText.text = $"{GetName()} was successful!";
        }
        else
        {
            _model.dialogueText.text = $"{GetName()} was blocked!";
        }

        var isDead = result.Item2;

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            _model.battleSystem.SetState(_model.finishState);
        }
        else
        {
            _model.battleSystem.SetState(_model.nextState);
        }

        Destroy(gameObject);
    }

    public override string GetName()
    {
        return "Head Butt";
    }
}
