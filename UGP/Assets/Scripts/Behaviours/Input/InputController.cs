using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public abstract class InputController : MonoBehaviour
    {
        public abstract void Move(float x, float y);
    }

}