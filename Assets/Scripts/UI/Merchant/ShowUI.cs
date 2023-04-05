using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using UnityEngine.InputSystem;

public class ShowUI : MonoBehaviour
{
    [SerializeField]
    GameObject self, canvas2, canvas3, hologramObj;
    GameObject hologram;
    List<GameObject> canvases = new List<GameObject>();
    public float floatSpeed = 1f;  // The speed of the floating motion
    public float floatHeight = 0.5f; // The maximum height of the float
    private Vector3 startPos, startHoloPos;
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
    ClickSound cs;
    [SerializeField]
    TMP_Text descriptionUI; 

    private void Start()
    {
        cs=GetComponent<ClickSound>();
        canvases.Add(self);
        canvases.Add(canvas2);
        canvases.Add(canvas3);
        startPos = self.transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        gun = GameObject.FindGameObjectWithTag("Gun");
        m_InputActionAsset = player.GetComponent<PlayerInput>().actions;
        m_InputActionAsset["Interact"].performed += ctx => SelectCurrentItem();
        //descriptionUI = self.transform.GetChild(0).GetComponentInChildren<TMP_Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            self.SetActive(true);
            if (hologramObj != null) {
                hologramObj.SetActive(true);
            }
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
            if (hologramObj != null)
            {
                hologramObj.SetActive(false);
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
            float newYH = startHoloPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
           /* if (hologramObj != null)
            {
                hologramObj.transform.position = new Vector3(startHoloPos.x, newYH, startHoloPos.z);
            }*/
            self.transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
        else {
            self.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
            if (hologramObj != null)
            {
                hologramObj.transform.position = new Vector3(startHoloPos.x, startHoloPos.y, startHoloPos.z);
            }
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
        cs.Click();
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
    public void SetText(string description)
    {// set the description of the config 
        if (description == "SOLD OUT!") {
            descriptionUI.text = description;
            return;
        }
        descriptionUI.text = description + "\n" + "cost: " + data.merchantPrices[description] + "pts";

        this.description = description;
        if (data.holograms.ContainsKey(description)) {
            hologram = data.holograms[description];
            hologramObj = GameObject.Instantiate(hologram);
            hologramObj.transform.SetParent(transform);
            hologramObj.transform.SetAsLastSibling();
            hologramObj.transform.localPosition = new Vector3(0, 1.4f, 0);//below
            //hologramObj.transform.localPosition = new Vector3(0, 3.5f, 0);// above
            startHoloPos = hologramObj.transform.position;
            hologramObj.SetActive(false);


        }
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
