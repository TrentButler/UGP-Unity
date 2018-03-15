using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [System.Serializable]
    public struct State
    {
        public int scores;
        public string name;
        public override string ToString()
        {
            return "current state is " + name + " high score is " + scores.ToString();
        }
    }

    public State c_state;

    void Awake()
    {
        c_state = new State() { scores = 0, name = "init" };
    }

    public void OnGameStarted(UnityEngine.Object[] args)
    {
        var sender = args[0] as UGPEVENTS.TestBehaviour;
        if (sender == null)
            return;
        c_state = new State() { scores = sender.highscore, name = "start" };
        Debug.Log(c_state.ToString());
    }
}
