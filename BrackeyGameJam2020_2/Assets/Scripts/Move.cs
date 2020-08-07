using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour, IMove
{
    public virtual IEnumerator Execute(BattleDataModel model) {
        yield return null;
    }

    public virtual string GetName()
    {
        return "Move";
    }

    public virtual void OnExecuteFinished() { }
}
