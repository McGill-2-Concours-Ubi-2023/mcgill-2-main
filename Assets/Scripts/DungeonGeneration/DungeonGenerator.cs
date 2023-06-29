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
        //PLEASE DON'T LEAVE IT THERE, FOR DEBUG PURPOSES ONLY
        Application.targetFrameRate = 30;
        data.SetMonoInstance(this);
        if (newDungeonOnPlay)
        {
            await data.GenerateDungeon();
            PlaceRandomMerchant();
        } else
        {
            await data.LoadData();
        }
        GameManager.isLoading = false;
        GameObject.FindWithTag("Player").Trigger<IMainCharacterTriggers>(nameof(IMainCharacterTriggers.ResetInventory));
    }

    private async void PlaceRandomMerchant()
    {
       await Task.Yield();         
       data.PlaceMerchant();
    }
}
