using UnityEngine;

[CreateAssetMenu]
public class GlobalGameManager : ScriptableObject
{
    public void PrintInfo(string info)
    {
        Debug.Log(info);
    }

    public int TotalDamageTaken;
    public int TotalDeaths = 0;

    public void IncrementDeaths()
    {
        TotalDeaths++;
        if(TotalDeaths > 1)
        {
            AnnouncerString("DOUBLE KILL");
            //raise a event
        }
        if(TotalDeaths > 4)
        {
            AnnouncerString("KILIMANJARO");
            
        }
    }

    public void AnnouncerString(string value)
    {
        Debug.Log(value);
    }
}
