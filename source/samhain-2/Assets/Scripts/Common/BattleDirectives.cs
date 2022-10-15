using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "New Battle Directives", menuName = "Create New Battle Directives", order = 0)]
    public class BattleDirectives : ScriptableObject
    {
        public List<GameObject> Characters;
        public SerializableDictionary<string, int> CharacterHealth;
        public SerializableDictionary<string, CharacterDeckDirective> CharacterDecks;
        public List<GameObject> Enemies;
    }
}