using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassController : MonoBehaviour
{
    private Renderer _renderer;
    public GrassInteractor[] interactors;
    private static int maxNumAgents = 5;
    private int[] idData;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        idData = new int[maxNumAgents];
        //Initialize all interactors to 0, in case default shader values modified;
        for (int i = 1; i <= maxNumAgents; i++)
        {
            _renderer.sharedMaterial.SetVector("_Interactor_" + i + "Position", Vector3.zero);
            _renderer.sharedMaterial.SetFloat("_InfluenceRadius_" + i, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Update ids data
        for(int i = 0; i < interactors.Length; i++)
        {
            idData[i] = interactors[i].id;           
        }

        //Check for unbound ids at runtime and update the collision state
        for (int i = 1; i <= idData.Length; i++)
        {
            if (!idData.ToList().Contains(i))
            {
                _renderer.sharedMaterial.SetVector("_Interactor_" + i + "Position", Vector3.zero);
                _renderer.sharedMaterial.SetFloat("_InfluenceRadius_" + i, 0);
            }
        }
        //Check for bound ids at runtime and update the collision state
        foreach (GrassInteractor interactor in interactors)
        {
            var position = interactor.gameObject.transform.position;
            int id = interactor.id;
            _renderer.sharedMaterial.SetVector("_Interactor_" + id + "Position", position);
            _renderer.sharedMaterial.SetFloat("_InfluenceRadius_" + id, interactor.influenceRadius);                  
        }        
    }
}
