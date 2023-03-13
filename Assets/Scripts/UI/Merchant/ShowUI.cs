using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowUI : MonoBehaviour
{
    [SerializeField]
    GameObject self, canvas2, canvas3;
    List<GameObject> canvases = new List<GameObject>();
    public float floatSpeed = 1f;  // The speed of the floating motion
    public float floatHeight = 0.5f; // The maximum height of the float
    private Vector3 startPos;
    private bool startFloat = false; 
    private void Start()
    {
        canvases.Add(self);
        canvases.Add(canvas2);
        canvases.Add(canvas3);
        startPos = self.transform.position; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            foreach (GameObject c in canvases) {
                c.SetActive(true);
                startFloat = true;
            }
            
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
        if (Input.GetKeyDown(KeyCode.E) && startFloat) {
            SelectCurrentConfiguration(); 
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

    private void SelectCurrentConfiguration() {
        // TODO: player selected this room config
    }

    private void SetText(string description) {// set the description of the config 
        self.transform.GetChild(0).GetComponentInChildren<TMP_Text>().text = description; 
    }
}
