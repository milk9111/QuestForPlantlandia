using System;
using System.Collections;

public interface IMove
{
    IEnumerator Execute(BattleDataModel model);
    string GetName();
    void OnExecuteFinished();
}
