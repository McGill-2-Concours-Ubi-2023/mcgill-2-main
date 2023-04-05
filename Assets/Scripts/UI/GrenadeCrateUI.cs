using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrenadeCrateUI : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    public ISimpleInventory<SimpleCollectible> SimpleCollectibleInventory;
    [SerializeField]
    private TMP_Text grenadeCount, crateCount; 


    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    public void UpdateGrenadeUI(int count) {
        if (count > 9)
        {
            grenadeCount.text = count.ToString();
        }
        else {
            grenadeCount.text = "0" + count.ToString();
        }
    }

    public void UpdateCrateUI(int count) {
        if (count > 9)
        {
            crateCount.text = count.ToString();
        }
        else {
            crateCount.text = "0" + count.ToString();
        }
    }
}
