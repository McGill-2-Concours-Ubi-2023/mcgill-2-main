using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using UnityEngine.InputSystem;

public class ShowUI : MonoBehaviour
{
    [SerializeField]
    GameObject self, canvas2, canvas3;
    List<GameObject> canvases = new List<GameObject>();
    public float floatSpeed = 1f;  // The speed of the floating motion
    public float floatHeight = 0.5f; // The maximum height of the float
    private Vector3 startPos;
    private bool startFloat = false;
    [SerializeField]
    Configuration configs;
    [SerializeField]
    MerchantData data; 
    string description;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject gun;
    int points = 10000;
    private bool soldout = false;
    private InputActionAsset m_InputActionAsset;

    private void Start()
    {
        canvases.Add(self);
        canvases.Add(canvas2);
        canvases.Add(canvas3);
        startPos = self.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        gun = GameObject.FindGameObjectWithTag("Gun");
        m_InputActionAsset = player.GetComponent<PlayerInput>().actions;
        m_InputActionAsset["Interact"].performed += ctx => SelectCurrentItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            self.SetActive(true);
            startFloat = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            foreach (GameObject c in canvases)
            {
                c.SetActive(false); ;
                startFloat = false;
            }
        }
    }

    private void Update()
    {
        if (startFloat)
        {
            Float(true);
        }
        else {
            Float(false); 
        }
        if ((Input.GetKeyDown(KeyCode.E))) {
            SelectCurrentItem(); 
        }
    }

    private void Float(bool shouldFloat) {
        if (shouldFloat)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            self.transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
        else {
            self.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
        }
        
    }

    /*private void SelectCurrentConfiguration() {
        // TODO: player selected this room config
        if (configs.configurations.ContainsKey(description)) {
            {
                List<GameObject> roomPrefabs = configs.configurations[description];
                int count = roomPrefabs.Count;
                GameObject prefab = roomPrefabs[Random.Range(0, count + 1)];
                // TODO: replace the room with this prefab; 
                
            }
        }
    }*/

    private void SelectCurrentItem()
    {
        // TODO: player selected this item
        if (startFloat && data.merchantPrices.ContainsKey(description) && !soldout)
        {
            {
                //TODO: check if player has enough points 
                if (points > data.merchantPrices[description]) {
                    points = points -= data.merchantPrices[description];
                    PickMethod(data.merchantMethods[description]);
                    soldout = true;
                    SetText("SOLD OUT!");
                }

            }
        }
    }
    public void SetText(string description) {// set the description of the config 
        self.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = description;
        this.description = description;
    }

    private void PickMethod(string method)
    {
        MethodInfo method1 = GetType().GetMethod(method);

        method1.Invoke(this, null);
    }

    public void GainOneHealth() {
        player.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.GainHealth), 1);
    }

    public void GainOneMaxHealth() {
        player.Trigger<IHealthTriggers, float>(nameof(IHealthTriggers.IncreaseMaxHealth), 1);
    }

    public void IncraseFireRateDouble() {
        gun.Trigger<IGunTriggers, float>(nameof(IGunTriggers.IncreaseFireRate), 2);
    }
    public void IncraseFireOnePointFive() {
        gun.Trigger<IGunTriggers, float>(nameof(IGunTriggers.IncreaseFireRate), 1.5f);
    }
    public void ChangeBullet() {
        gun.Trigger<IGunTriggers>(nameof(IGunTriggers.ChangeBullet));
    }
  /*  private void GainOneMaxGrenade() {
        this.gameObject.Trigger<IGrenadeTriggers, int>(nameof(IGrenadeTriggers.IncreaseMaxGrenade), 1);
    }*/
}
