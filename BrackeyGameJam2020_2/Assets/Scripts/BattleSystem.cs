using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    public string overridingPlayer;
    public string overridingEnemy;

    public ActionHUD actionHUD;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public Text dialogueText;

    public GameObject levelUpBanner;

    public GameObject geminiObj;
    public GameObject ferbObj;
    public GameObject violaObj;

    public GameObject itchObj;
    public GameObject mushObj;
    public GameObject snipObj;
    public GameObject squirtObj;
    public GameObject woodyObj;

    public List<string> enemies;

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
        levelUpBanner.SetActive(false);

        actionHUD.Disable();

        var playerName = GlobalControl.instance.playerSave.unitName;
        if (!string.IsNullOrEmpty(overridingPlayer))
        {
            playerName = overridingPlayer;
        }

        GameObject player = null;
        switch(playerName)
        {
            case "Gemini":
                player = Instantiate(geminiObj, playerBattleStation);
                break;
            case "Ferb":
                player = Instantiate(ferbObj, playerBattleStation);
                break;
            case "Viola":
                player = Instantiate(violaObj, playerBattleStation);
                break;
        }

        playerUnit = player.GetComponent<Unit>();
        playerUnit.finishState = BattleState.Lost;

        if (string.IsNullOrEmpty(overridingPlayer))
        {
            Debug.Log("in here");
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

        var enemyName = enemies[Random.Range(0, enemies.Count)];
        if (GlobalControl.instance.enemySave != null && !string.IsNullOrEmpty(GlobalControl.instance.enemySave.unitName))
        {
            enemyName = GlobalControl.instance.enemySave.unitName;
        }

        if (!string.IsNullOrEmpty(overridingEnemy))
        {
            enemyName = overridingEnemy;
        }

        GameObject enemy = null;
        switch (enemyName)
        {
            case "Itch":
                enemy = Instantiate(itchObj, enemyBattleStation);
                break;
            case "Mush":
                enemy = Instantiate(mushObj, enemyBattleStation);
                break;
            case "Snip":
                enemy = Instantiate(snipObj, enemyBattleStation);
                break;
            case "Squirt":
                enemy = Instantiate(squirtObj, enemyBattleStation);
                break;
            case "Woody":
                enemy = Instantiate(woodyObj, enemyBattleStation);
                break;
        }

        var eu = enemy.GetComponent<Unit>();
        eu.finishState = BattleState.Won;

        eu.maxHP += GlobalControl.instance.dungeonSave.level;
        eu.unitLevel = GlobalControl.instance.dungeonSave.level;

        eu.currentHP = eu.maxHP;


        if (GlobalControl.instance.enemySave != null && !GlobalControl.instance.enemySave.isNew)
        {
            eu.unitName = GlobalControl.instance.enemySave.unitName;
            eu.unitLevel = GlobalControl.instance.enemySave.unitLevel;
            eu.experience = GlobalControl.instance.enemySave.experience;
            eu.experienceToNextLevel = GlobalControl.instance.enemySave.experienceToNextLevel;
            eu.damage = GlobalControl.instance.enemySave.damage;
            eu.maxHP = GlobalControl.instance.enemySave.maxHP;
            eu.currentHP = GlobalControl.instance.enemySave.currentHP;
        }

        enemyUnit = eu;

        dialogueText.text = "A wild " + enemyUnit.unitName + " appears...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        playerUnit.hud = playerHUD;
        enemyUnit.hud = enemyHUD;

        playerUnit.dialogueText = dialogueText;
        enemyUnit.dialogueText = dialogueText;

        foreach (var move in FindObjectsOfType<Move>())
        {
            if (_moves.ContainsKey(move.GetName()))
            {
                continue;
            }

            _moves.Add(move.GetName(), move);
        }

        _model = new BattleDataModel(this, enemyUnit, playerUnit, enemyHUD, playerHUD, dialogueText, BattleState.PlayerTurn, BattleState.Won);

        yield return new WaitForSeconds(2f);

        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        if (playerUnit.SkipTurn())
        {
            StartCoroutine(Asleep(playerUnit.unitName, BattleState.EnemyTurn));
            return;
        }

        actionHUD.Enable();
        dialogueText.text = "Choose an action:";
    }

    private IEnumerator Asleep(string unitName, BattleState nextState)
    {
        dialogueText.text = $"{unitName} is asleep!";
        yield return new WaitForSeconds(0.6f);

        SetState(nextState);
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
                dialogueText.text = $"{playerUnit.unitName} tried to use {moveName}. It was not effective!";
                Debug.LogError($"Move {moveName} was not in the dictionary!");
                yield return new WaitForSeconds(2f);
                SetState(BattleState.EnemyTurn);
            } else
            {
                dialogueText.text = $"{playerUnit.unitName} uses {moveName}.";

                yield return new WaitForSeconds(2f);

                SetModelSource(playerUnit, playerHUD);
                SetModelTarget(enemyUnit, enemyHUD);
                SetModelStates(BattleState.EnemyTurn, BattleState.Won);

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

    private void SetModelStates(BattleState nextState, BattleState finishState)
    {
        _model.nextState = nextState;
        _model.finishState = finishState;
    }

    public void SetState(BattleState state)
    {
        this.state = state;

        enemyHUD.SetHP(enemyUnit.currentHP);
        playerHUD.SetHP(playerUnit.currentHP);

        switch (this.state)
        {
            case BattleState.Start:
                StartCoroutine(SetupBattle());
                break;
            case BattleState.PlayerTurn:
                StartCoroutine(enemyUnit.OnTurn(delegate
                {
                    StartCoroutine(playerUnit.OnTurn(delegate
                    {
                        PlayerTurn();
                    }));
                }));
                break;
            case BattleState.EnemyTurn:
                StartCoroutine(playerUnit.OnTurn(delegate
                {
                    StartCoroutine(enemyUnit.OnTurn(delegate
                    {
                        EnemyTurn();
                    }));
                }));
                break;
            case BattleState.Won:
                StartCoroutine(WonBattle());
                break;
            case BattleState.Lost:
                StartCoroutine(LostBattle());
                break;
        }
    }

    void EnemyTurn()
    {
        if (enemyUnit.SkipTurn())
        {
            StartCoroutine(Asleep(enemyUnit.unitName, BattleState.PlayerTurn));
            return;
        }

        StartCoroutine(ExecuteEnemyAction(enemyUnit.enemyMoves[Random.Range(0, enemyUnit.enemyMoves.Count)]));
    }

    private IEnumerator ExecuteEnemyAction(string moveName)
    {
        if (state != BattleState.EnemyTurn)
        {
            yield return null;
        }
        else
        {
            actionHUD.Disable();
            if (!_moves.ContainsKey(moveName))
            {
                dialogueText.text = $"{enemyUnit.unitName} tried to use {moveName}. It was not very effective...";
                Debug.LogError($"Move {moveName} was not in the dictionary!");
                yield return new WaitForSeconds(2f);
                SetState(BattleState.PlayerTurn);
            }
            else
            {
                dialogueText.text = $"{enemyUnit.unitName} uses {moveName}.";

                yield return new WaitForSeconds(2f);

                SetModelTarget(playerUnit, playerHUD);
                SetModelSource(enemyUnit, enemyHUD);
                SetModelStates(BattleState.PlayerTurn, BattleState.Lost);

                var move = Instantiate(_moves[moveName].gameObject, enemyUnit.launcher);

                StartCoroutine(move.GetComponent<Move>().Execute(_model));
            }
        }
    }

    IEnumerator WonBattle()
    {
        dialogueText.text = "You won the battle!";
        yield return new WaitForSeconds(1f);

        if (playerUnit.LevelUp(enemyUnit.experience))
        {
            levelUpBanner.SetActive(true);
            yield return new WaitForSeconds(3f);
        }

        BackToDungeon();
    }

    IEnumerator LostBattle()
    {
        dialogueText.text = "You were defeated...";
        yield return new WaitForSeconds(1f);

        playerUnit.isDead = true;

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
        GlobalControl.instance.playerSave.isDead = playerUnit.isDead;

        GlobalControl.instance.enemySave.isNew = true;

        GlobalControl.instance.rewindStack.Peek().playerUnit = new UnitSave(playerUnit, false);

        PlayerPrefs.SetString("NextScene", "test_v2");
        GlobalControl.instance.currentScene = "test_v2";

        SceneManager.LoadScene("loading_screen");
    }
}
