using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    [SerializeField]
    GameObject canvas1, canvas2, canvas3;
    [SerializeField]
    MerchantData data;
    [SerializeField]
    GameObject player;

    public void AssignRandomItem() {
        List<int> itemIndices = new List<int>();
        while(itemIndices.Count < 3){
            int index = Random.Range(0, data.descriptions.Count);
            if (!itemIndices.Contains(index)) {
                itemIndices.Add(index);
            }
        }
        canvas1.GetComponentInParent<ShowUI>().SetText(data.descriptions[itemIndices[0]]);
        canvas2.GetComponentInParent<ShowUI>().SetText(data.descriptions[itemIndices[1]]);
        canvas3.GetComponentInParent<ShowUI>().SetText(data.descriptions[itemIndices[2]]);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        AssignRandomItem();
    }
}
