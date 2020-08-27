using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBolt : Move
{

    public Transform target;
    public Transform start;

    public float speed = 1.0F;

    public bool canMove = false;

    public int damage = 3;

    private float startTime;

    private float journeyLength;

    private BattleDataModel _model;

    private SpriteRenderer _renderer;

    private Collider2D _collider;

    private void Init()
    {
        canMove = false;
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = false;
        _collider = GetComponent<Collider2D>();
        _collider.enabled = true;

        start = transform;

        startTime = Time.time;

        var t = new Vector3(0, 0, 0);
        if (target != null)
        {
            t = new Vector3(target.position.x, target.position.y, 0);
        }

        journeyLength = Vector3.Distance(start.position, t);
    }

    void Update()
    {
        if (!canMove || journeyLength == 0)
        {
            return;
        }

        float distCovered = (Time.time - startTime) * speed;

        float fractionOfJourney = distCovered / journeyLength;

        var t = new Vector3(0, 0, 0);
        if (target != null)
        {
            t = new Vector3(target.position.x, target.position.y, 0);
        }

        transform.position = Vector3.Lerp(start.position, t, fractionOfJourney);
    }

    public override IEnumerator Execute(BattleDataModel model)
    {
        Init();

        _renderer.enabled = true;
        canMove = true;
        target = model.targetUnit.transform;
        _model = model;

        if (target.position.x < transform.position.x)
        {
            _renderer.flipX = true;
        }

        yield return null;
    }

    IEnumerator Finish()
    {
        var result = _model.targetUnit.TakeDamage(_model.sourceUnit.damage + damage);
        _renderer.enabled = false;

        if (result.Item1)
        {
            _model.targetHUD.SetHP(_model.targetUnit.currentHP);
            _model.dialogueText.text = "Lightning Bolt was successful!";
        }
        else
        {
            _model.dialogueText.text = "Lightning Bolt was blocked!";
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_model == null)
        {
            return;
        }

        if (string.Equals(collision.gameObject.name, _model.targetUnit.name))
        {
            canMove = false;
            StartCoroutine(Finish());
        }
    }

    public override string GetName()
    {
        return "Lightning Bolt";
    }
}
