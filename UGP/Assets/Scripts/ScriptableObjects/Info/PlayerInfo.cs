using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UGP
{ 
[CreateAssetMenu(fileName = "PlayerInfo", menuName = "PlayerInfo", order = 10)]
public class PlayerInfo : ScriptableObject
{
    public string PlayerName;
    public Color PlayerColor;
}
}