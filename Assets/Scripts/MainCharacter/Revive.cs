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
    [SerializeField]
    private GrenadeCrateUI gcUI;

    private void Start()
    {
        if (!revivePanel)
        {
            revivePanel = GameObject.FindObjectOfType<Canvas>()?.transform.Find("RevivePanel")?.gameObject;
        }
        if (!revivePanel)
        {
            Debug.LogError("RevivePanel not found");
            UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.FatalError);
        }
        gcUI = GameObject.FindObjectOfType<GrenadeCrateUI>();
        gameObject.GetComponent<Health>().OnDeath += RevivePlayerPromt;
        scoreSys = GameObject.FindObjectOfType<ScoringSystem>();
    }
    public void RevivePlayerPromt() {
        Time.timeScale = 0; 
        Debug.Log("promt called");
        revivePanel.SetActive(true);
        TMP_Text text = revivePanel.GetComponentInChildren<TMP_Text>();
        halfScore = (int)scoreSys.currScore / 2;
        text.text = "cost: "+ halfScore + "pts";
        EventSystem.current.SetSelectedGameObject(button1);
    }

    public void RevivePlayer() {
        if (halfScore == -1) return;
        Time.timeScale = 1;
        if (!scoreSys.TryPurchase(halfScore)) {
            Debug.LogError("scoreSys doesn't have enough score??? How?");
        }
        scoreSys.UpdateScore();
        MainCharacterController mcc = GetComponent<MainCharacterController>();
        mcc.ResetInventory();
        MainCharacterController.SimpleCollectibleInventory.AddInBulk(SimpleCollectible.CratePoint, 5);
        MainCharacterController.SimpleCollectibleInventory.AddInBulk(SimpleCollectible.Grenade, 2);
        gcUI.UpdateGrenadeUI(MainCharacterController.SimpleCollectibleInventory.GetCount(SimpleCollectible.Grenade), +MainCharacterController.SimpleCollectibleInventory.GetMax(SimpleCollectible.Grenade)); ;
        gameObject.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.GainHealth), 100);
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
