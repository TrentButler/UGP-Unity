using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    [CreateAssetMenu(fileName = "AllScenesInBuild", menuName = "ScenesInBuild")]
    public class ScenesInBuildScriptableObject : ScriptableObject
    {
        public List<string> scenes;
    }
}

