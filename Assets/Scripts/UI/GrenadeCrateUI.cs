using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GrenadeCrateUI : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    private TMP_Text grenadeCount, crateCount; 

    public void UpdateGrenadeUI(int count, int max) {
        /*if (count > 9)
        {
            grenadeCount.text = count.ToString() + "/" + max.ToString();
        }
        else {
            grenadeCount.text = "0" + count.ToString() + "/" + max.ToString();
        }*/
        grenadeCount.text = count.ToString().PadLeft(2, '0') + "/" + max.ToString().PadLeft(2, '0');
    }

    public void UpdateCrateUI(int count, int max) {
       /* if (count > 9)
        {
            crateCount.text = count.ToString() + "/" + max.ToString();
        }
        else {
            crateCount.text = "0" + count.ToString() + "/" + max.ToString();
        }*/
        crateCount.text = count.ToString().PadLeft(2,'0') + "/" + max.ToString().PadLeft(2, '0');
    }
}
