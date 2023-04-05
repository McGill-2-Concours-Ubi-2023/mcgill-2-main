using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    GameObject heartPrefab;
    [SerializeField]
    List<GameObject> hearts = new List<GameObject>();
    [SerializeField]
    Health playerHealth;

    private void Start()
    {
        playerHealth.OnHealthChange += HealthChange;
        playerHealth.OnDeath += Death;
    }
    public void GenerateHearts(int number) {
        for (int i = 0; i < number; i++)
        {
            GameObject heart = GameObject.Instantiate(heartPrefab);
            heart.transform.SetParent(transform);
            hearts.Add(heart);
        }
    }

    public void HealthChange(float change, float currentHealth) {
        if (change < 0)
        {
            if (hearts.Count <= 0) return;
            for (int i = 0; i < -change; i++) {
                GameObject.Destroy(hearts[0]);
                hearts.RemoveAt(0);
            }
            
        }
        else {
            GenerateHearts((int)change);
        }    
    }

    public void Death() {
        for (int i = 0; i < hearts.Count; i++) {
            GameObject.Destroy(hearts[0]);
            hearts.RemoveAt(0);
        }
    }

    private void Update()
    {/* test code
        if (Input.GetKeyDown(KeyCode.A)) {
            playerHealth.GainHealth(2);
        }*/
    }
}
