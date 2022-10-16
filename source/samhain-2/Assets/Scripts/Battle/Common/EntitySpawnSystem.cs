using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.Events;
using CharacterActions =
    System.Tuple<UnityEngine.Events.UnityAction<UnityEngine.GameObject, UnityEngine.GameObject>,
        UnityEngine.Events.UnityAction<UnityEngine.GameObject, UnityEngine.GameObject>,
        UnityEngine.Events.UnityAction<UnityEngine.GameObject>, UnityEngine.Events.UnityAction<UnityEngine.GameObject>>;

public class EntitySpawnSystem : MonoBehaviour
{
    public BattleDirectives BattleDirectives;
    public List<GameObject> Characters = new();
    public List<GameObject> Enemies = new();
    public Vector2 CharacterOffsetRanges;
    public Vector2 EnemyOffsetRanges;
    public GameObject CharacterParentObject;
    public GameObject EnemyParentObject;
    public GameObject StatusOverlayParent;
    public GameObject StatusOverlayPrefab;
    public TargetingSystem TargetingSystem;
    public TurnSystem TurnSystem;
    public GameObject HandParent;
    public BattleTransitionSystem BattleTransitionSystem;

    private List<CharacterActions> characterActions = new();
    private List<UnityAction<GameObject, GameObject>> enemyActions = new();
    public HandAnchorPositioningSystem HandAnchorPositioningSystem;

    private void Start()
    {
        PopulateBattleDirectives();
        Characters = BattleDirectives.Characters.ToList()
            .Select(element => Instantiate(element, CharacterParentObject.transform)).ToList();
        Characters.ForEach(element =>
            element.GetComponent<EntityHealth>()._currentHealth =
                BattleDirectives.CharacterHealth[element.GetComponent<EntityHealth>().EntityName]);
        Enemies = BattleDirectives.Enemies.ToList().Select(element => Instantiate(element, EnemyParentObject.transform))
            .ToList();
        Characters.ForEach(element => element.GetComponent<EntityTargeting>().TargetingSystem = TargetingSystem);
        Characters.ForEach(element =>
            element.GetComponent<CharacterDeck>().DrawPile =
                BattleDirectives.CharacterDecks[element.GetComponent<EntityHealth>().EntityName].Cards);
        Characters.ForEach(element => element.GetComponent<CharacterDeck>().HandParent = HandParent);
        Characters.ForEach(element => element.GetComponent<CharacterDeck>().TargetingSystem = TargetingSystem);
        Enemies.ForEach(element => element.GetComponent<EntityTargeting>().TargetingSystem = TargetingSystem);
        Enemies.ForEach(element => element.GetComponent<EntityHealth>().ResetHealth());
        PositionCharacters();
        PositionEnemies();
        InitializeStatusOverlays();
        TargetingSystem.ActiveTurn = Characters[0];
        TurnSystem.TurnSequence.AddRange(Characters);
        TurnSystem.TurnSequence.AddRange(Enemies);
        HandAnchorPositioningSystem.Initialize();
        TurnSystem.Init();
        characterActions = Characters.Select(InitializeCharacterEvents).ToList();
        enemyActions = Enemies.Select(InitializeEnemyEvents).ToList();
        Characters.ForEach(element => element.GetComponent<CharacterDeck>().Initialize());
        Enemies.ForEach(element => element.GetComponent<BaseEnemyAI>().Characters = Characters);
        TurnSystem.OnTurnStart.AddListener(TargetingSystem.UpdateTurn);
        TurnSystem.TurnSequence[0].GetComponent<CharacterDeck>().DrawCardsStartTurn(null, TurnSystem.TurnSequence[0]);
        BattleTransitionSystem.OnStartBattle.Invoke();
    }

    private void OnDestroy()
    {
        characterActions.ForEach(UnsubscribeCharacterEvents);
        enemyActions.ForEach(UnsubscribeEnemyEvents);
        TurnSystem.OnTurnStart.RemoveListener(TargetingSystem.UpdateTurn);
    }

    private void PopulateBattleDirectives()
    {
        // var DefaultDirectives = GetComponent<DefaultDirectives>();
        // BattleDirectives.CharacterDecks =DefaultDirectives.CharacterDecks;
        // BattleDirectives.CharacterHealth = DefaultDirectives.CharacterHealth;
    }

    public void PositionCharacters()
    {
        var totalOffsetWidth = CharacterOffsetRanges.x * 2;
        var totalOffsetHeight = CharacterOffsetRanges.y * 2;

        var offsetWidth = totalOffsetWidth / (Characters.Count + 1);
        var offsetHeight = totalOffsetHeight / (Characters.Count + 1);

        var offsetBase = CharacterParentObject.transform.position;
        offsetBase.x += CharacterOffsetRanges.x;
        offsetBase.y += CharacterOffsetRanges.y;
        foreach (var character in Characters)
        {
            offsetBase.x -= offsetWidth;
            offsetBase.y -= offsetHeight;
            character.transform.position = offsetBase;
        }
    }

    public void PositionEnemies()
    {
        var totalOffsetWidth = EnemyOffsetRanges.x * 2;
        var totalOffsetHeight = EnemyOffsetRanges.y * 2;

        var offsetWidth = totalOffsetWidth / (Enemies.Count + 1);
        var offsetHeight = totalOffsetHeight / (Enemies.Count + 1);

        var offsetBase = EnemyParentObject.transform.position;
        offsetBase.x -= EnemyOffsetRanges.x;
        offsetBase.y += EnemyOffsetRanges.y;
        foreach (var character in Enemies)
        {
            offsetBase.x += offsetWidth;
            offsetBase.y -= offsetHeight;
            character.transform.position = offsetBase;
        }
    }

    public void InitializeStatusOverlays()
    {
        Characters.ForEach(InitializeStatusOverlay);
        Enemies.ForEach(InitializeStatusOverlay);
    }

    public void InitializeStatusOverlay(GameObject entity)
    {
        var overlay = Instantiate(StatusOverlayPrefab, StatusOverlayParent.transform);
        overlay.GetComponent<EntityStatusUI>().TargetHealth = entity.GetComponent<EntityHealth>();
    }

    public CharacterActions InitializeCharacterEvents(GameObject character)
    {
        UnityAction<GameObject, GameObject> drawAction = (arg0, o) =>
            character.GetComponent<CharacterDeck>().DrawCardsStartTurn(arg0, o);
        TurnSystem.OnTurnStart.AddListener(drawAction);

        UnityAction<GameObject, GameObject> manaAction = (arg0, o) =>
            character.GetComponent<CharacterMana>().RefillManaStartTurn(arg0, o);
        TurnSystem.OnTurnStart.AddListener(manaAction);

        UnityAction<GameObject> discardAction = arg0 => character.GetComponent<CharacterDeck>().DiscardEndOfTurn(arg0);
        TurnSystem.OnTurnEnd.AddListener(discardAction);

        UnityAction<GameObject> deathAction = arg0 => BattleTransitionSystem.CheckForBattleEnd(arg0);
        character.GetComponent<EntityHealth>().OnEntityDeath.AddListener(deathAction);

        CharacterActions actions = new(drawAction, manaAction, discardAction, deathAction);
        return actions;
    }

    public UnityAction<GameObject, GameObject> InitializeEnemyEvents(GameObject enemy)
    {
        UnityAction<GameObject, GameObject>
            action = (arg0, o) => enemy.GetComponent<BaseEnemyAI>().PerformTurn(arg0, o);
        TurnSystem.OnTurnStart.AddListener(action);
        return action;
    }

    public void UnsubscribeCharacterEvents(CharacterActions actions)
    {
        TurnSystem.OnTurnStart.RemoveListener(actions.Item1);
        TurnSystem.OnTurnStart.RemoveListener(actions.Item2);
        TurnSystem.OnTurnEnd.RemoveListener(actions.Item3);
    }

    public void UnsubscribeEnemyEvents(UnityAction<GameObject, GameObject> actions)
    {
        TurnSystem.OnTurnStart.RemoveListener(actions);
    }
}