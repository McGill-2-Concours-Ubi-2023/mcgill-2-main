using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Revive : MonoBehaviour
{
    [SerializeField]
    private GameObject revivePanel;
    [SerializeField]
    private ScoringSystem scoreSys;
    private int halfScore = -1;
    [SerializeField]
    private GameObject button1, button2;
    private GameObject selectedButton;

    private void Start()
    {
        //revivePanel = GameObject.Find("RevivePanel");
        gameObject.GetComponent<Health>().OnDeath += RevivePlayerPromt;
        scoreSys = GameObject.FindObjectOfType<ScoringSystem>();
    }
    public void RevivePlayerPromt() {
        Time.timeScale = 0; 
        Debug.Log("promt called");
        revivePanel.SetActive(true);
        TMP_Text text = revivePanel.GetComponentInChildren<TMP_Text>();
        halfScore = (int)scoreSys.currScore / 2;
        text.text = "Do you want to revive with half of your points?\n" + "cost: "+ halfScore + "pts";
        EventSystem.current.SetSelectedGameObject(button1);
    }

    public void RevivePlayer() {
        if (halfScore == -1) return;
        Time.timeScale = 1;
        if (!scoreSys.TryPurchase(halfScore)) {
            Debug.LogError("scoreSys doesn't have enough score??? How?");
        }
        scoreSys.UpdateScore();
        GetComponent<MainCharacterController>().ResetInventory();
        gameObject.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.GainHealth), 100);
        //TODO: change the number of grenades and crates to the starting game values;
        revivePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void EndGame() {
        Time.timeScale = 1;
        revivePanel.SetActive(false);
        GetComponent<MainCharacterController>().OnPlayerDeath();
        EventSystem.current.SetSelectedGameObject(null);
    }
}
