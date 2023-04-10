using UnityEngine;

public class DifficultyDisplayManager : MonoBehaviour
{
    public void NextDifficulty()
    {
        GameManager.Instance.NextDifficulty();
    }
    
    public void PreviousDifficulty()
    {
        GameManager.Instance.PreviousDifficulty();
    }
}
