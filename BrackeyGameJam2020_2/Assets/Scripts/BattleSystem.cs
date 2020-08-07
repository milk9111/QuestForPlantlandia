using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    PlayerTurn,
    Enemyturn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public ActionHUD actionHUD;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public Text dialogueText;

    public GameObject playerObj;
    public GameObject enemyObj;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public BattleState state;

    public Unit playerUnit;
    public Unit enemyUnit;

    private BattleDataModel _model;

    private IDictionary<string, Move> _moves = new Dictionary<string, Move>();

    void Start()
    {
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        actionHUD.Disable();
        var player = Instantiate(playerObj, playerBattleStation);
        playerUnit = player.GetComponent<Unit>();

        if (!GlobalControl.instance.playerSave.isNew)
        {
            playerUnit.unitName = GlobalControl.instance.playerSave.unitName;
            playerUnit.unitLevel = GlobalControl.instance.playerSave.unitLevel;
            playerUnit.experience = GlobalControl.instance.playerSave.experience;
            playerUnit.experienceToNextLevel = GlobalControl.instance.playerSave.experienceToNextLevel;
            playerUnit.damage = GlobalControl.instance.playerSave.damage;
            playerUnit.maxHP = GlobalControl.instance.playerSave.maxHP;
            playerUnit.currentHP = GlobalControl.instance.playerSave.currentHP;
            playerUnit.offensiveMove1Name = GlobalControl.instance.playerSave.offensiveMove1Name;
            playerUnit.offensiveMove2Name = GlobalControl.instance.playerSave.offensiveMove2Name;
            playerUnit.defensiveMoveName = GlobalControl.instance.playerSave.defensiveMoveName;
        }

        actionHUD.SetText(playerUnit.offensiveMove1Name, playerUnit.offensiveMove2Name, playerUnit.defensiveMoveName);

        var enemy = Instantiate(enemyObj, enemyBattleStation);
        var eu = enemy.GetComponent<Unit>(); ;
        if (GlobalControl.instance.enemySave != null && !GlobalControl.instance.enemySave.isNew)
        {
            eu.unitName = GlobalControl.instance.enemySave.unitName;
            eu.unitLevel = GlobalControl.instance.enemySave.unitLevel;
            eu.experience = GlobalControl.instance.enemySave.experience;
            eu.experienceToNextLevel = GlobalControl.instance.enemySave.experienceToNextLevel;
            eu.damage = GlobalControl.instance.enemySave.damage;
            eu.maxHP = GlobalControl.instance.enemySave.maxHP;
            eu.currentHP = GlobalControl.instance.enemySave.currentHP;
            eu.offensiveMove1Name = GlobalControl.instance.enemySave.offensiveMove1Name;
            eu.offensiveMove2Name = GlobalControl.instance.enemySave.offensiveMove2Name;
            eu.defensiveMoveName = GlobalControl.instance.enemySave.defensiveMoveName;
        }

        enemyUnit = eu;

        dialogueText.text = "A mysterious " + enemyUnit.unitName + " appears...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        foreach (var move in FindObjectsOfType<Move>())
        {
            if (_moves.ContainsKey(move.GetName()))
            {
                continue;
            }

            _moves.Add(move.GetName(), move);
        }

        _model = new BattleDataModel(this, enemyUnit, playerUnit, enemyHUD, playerHUD, dialogueText);

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        actionHUD.Enable();
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton1()
    {
        StartCoroutine(ExecutePlayerAction(playerUnit.offensiveMove1Name));
    }

    public void OnAttackButton2()
    {
        StartCoroutine(ExecutePlayerAction(playerUnit.offensiveMove2Name));
    }

    public void OnDefenseButton()
    {
        StartCoroutine(ExecutePlayerAction(playerUnit.defensiveMoveName));
    }

    public void DeactivateActionHUD()
    {
        actionHUD.Disable();
    }

    public void ActivateActionHUD()
    {
        actionHUD.Enable();
    }

    private IEnumerator ExecutePlayerAction(string moveName)
    {
        if (state != BattleState.PlayerTurn)
        {
            yield return null;
        } else
        {
            actionHUD.Disable();
            if (!_moves.ContainsKey(moveName))
            {
                dialogueText.text = "That move was not effective!";
                Debug.LogError($"Move {moveName} was not in the dictionary!");
                yield return new WaitForSeconds(2f);
                SetState(BattleState.Enemyturn);
            } else
            {
                SetModelSource(playerUnit, playerHUD);
                SetModelTarget(enemyUnit, enemyHUD);

                var move = Instantiate(_moves[moveName].gameObject, playerUnit.launcher);

                StartCoroutine(move.GetComponent<Move>().Execute(_model));
            }
        }
    }

    private void SetModelSource(Unit sourceUnit, BattleHUD sourceHUD)
    {
        _model.sourceUnit = sourceUnit;
        _model.sourceHUD = sourceHUD;
    }

    private void SetModelTarget(Unit targetUnit, BattleHUD targetHUD)
    {
        _model.targetUnit = targetUnit;
        _model.targetHUD = targetHUD;
    }

    public void OnItemsButton()
    {
        if (state != BattleState.PlayerTurn)
        {
            return;
        }

        actionHUD.Disable();
        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack()
    {
        var isDead = enemyUnit.TakeDamage(playerUnit.damage);

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            SetState(BattleState.Won);
        }
        else
        {
            SetState(BattleState.Enemyturn);
        }
    }

    public void SetState(BattleState state)
    {
        this.state = state;

        switch (this.state)
        {
            case BattleState.Start:
                StartCoroutine(SetupBattle());
                break;
            case BattleState.PlayerTurn:
                PlayerTurn();
                break;
            case BattleState.Enemyturn:
                StartCoroutine(EnemyTurn());
                break;
            case BattleState.Won:
                StartCoroutine(WonBattle());
                break;
            case BattleState.Lost:
                StartCoroutine(LostBattle());
                break;
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        var isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            SetState(BattleState.Lost);
        } else
        {
            SetState(BattleState.PlayerTurn);
        }
    }

    IEnumerator WonBattle()
    {
        dialogueText.text = "You won the battle!";
        yield return new WaitForSeconds(1f);

        if (playerUnit.LevelUp(enemyUnit.experience))
        {
            Debug.Log("Level Up!");
        }

        BackToDungeon();
    }

    IEnumerator LostBattle()
    {
        dialogueText.text = "You were defeated...";
        yield return new WaitForSeconds(1f);

        BackToDungeon();
    }

    public void BackToDungeon()
    {
        GlobalControl.instance.playerSave.unitName = playerUnit.unitName;
        GlobalControl.instance.playerSave.unitLevel = playerUnit.unitLevel;
        GlobalControl.instance.playerSave.experience = playerUnit.experience;
        GlobalControl.instance.playerSave.experienceToNextLevel = playerUnit.experienceToNextLevel;
        GlobalControl.instance.playerSave.damage = playerUnit.damage;
        GlobalControl.instance.playerSave.maxHP = playerUnit.maxHP;
        GlobalControl.instance.playerSave.currentHP = playerUnit.currentHP;
        GlobalControl.instance.playerSave.offensiveMove1Name = playerUnit.offensiveMove1Name;
        GlobalControl.instance.playerSave.offensiveMove2Name = playerUnit.offensiveMove2Name;
        GlobalControl.instance.playerSave.defensiveMoveName = playerUnit.defensiveMoveName;
        GlobalControl.instance.playerSave.isNew = false;

        GlobalControl.instance.enemySave.isNew = true;

        GlobalControl.instance.rewindStack.Peek().playerUnit = new UnitSave(playerUnit, false);

        PlayerPrefs.SetString("NextScene", "test_v2");
        GlobalControl.instance.currentScene = "test_v2";

        SceneManager.LoadScene("loading_screen");
    }
}
