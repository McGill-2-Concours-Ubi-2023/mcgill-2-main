using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LeastSquares;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
public class Menu : MonoBehaviour
{
    public bool cursor;
    private SteamLeaderboard leaderboard;
    
    void Start()
    {
        /* if (!cursor)
         {
             Cursor.lockState = CursorLockMode.Locked;
             Cursor.visible = false;
         }
         else
             Cursor.visible = true;*/
        //UpdateLeaderboard();
    }

    private async void UpdateLeaderboard()
    {
        try
        {
            while(leaderboard == null)
            {
                leaderboard = FindObjectOfType<SteamLeaderboard>();
                await Task.Yield();              
            }
            leaderboard.SubmitScore(GameManager.score);
        } 
        catch
        {
            Debug.Log("Leaderboard not found");
        }
    }

    public void Shut()
    {
        Application.Quit();
    }

    public void URL(string url)
    {
        Application.OpenURL(url);
    }
    public void LoadScene(int i) {
        GameManager.Instance.LoadScene(i);
    }
    public void LoadScene(string s)
    {
        throw new System.NotImplementedException();
    }
}
