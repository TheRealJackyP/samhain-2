using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "NewCharacterDeckDirective", menuName = "Create CharacterDeckDirective", order = 0)]
    public class CharacterDeckDirective : ScriptableObject
    {
        public List<GameObject> Cards;
    }
}