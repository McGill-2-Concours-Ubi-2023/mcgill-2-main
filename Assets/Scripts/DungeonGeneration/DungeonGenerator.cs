using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.AI.Navigation;

[ExecuteInEditMode]
public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public DungeonData data;
    [SerializeField]
    private bool newDungeonOnPlay = false;
    public DungeonRoom roomToReplace;

    private async void Awake()
    {
        data.SetMonoInstance(this);
        if (newDungeonOnPlay)
        {
            await data.GenerateDungeon();
        } else
        {
            await data.LoadData();
        }
        //StartCoroutine(PlaceRandomMerchant());
        
        GameManager.isLoading = false;
        GameObject.FindWithTag("Player").Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.ResetInventory));
    }

    IEnumerator PlaceRandomMerchant()
    {
        yield return new WaitForEndOfFrame();
        data.PlaceMerchant();
    }
}
