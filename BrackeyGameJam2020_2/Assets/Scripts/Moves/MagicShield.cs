using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShield : Move
{
    public override IEnumerator Execute(BattleDataModel model)
    {
        Debug.Log("Executing Magic Shield");
        yield return null;
        model.battleSystem.SetState(BattleState.EnemyTurn);
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetName()
    {
        return "Magic Shield";
    }

}
