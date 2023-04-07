using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBg : MonoBehaviour
{
    Vector2 position; 
    Vector2 StartPos; 

    [SerializeField] int moveModifier;

    private void Start() {
        StartPos = transform.position;
    }

    private void Update() {
        Vector2 position = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        float posX = Mathf.Lerp(transform.position.x, StartPos.x + (position.x * moveModifier), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, StartPos.y + (position.y * moveModifier/2), 2f * Time.deltaTime);
        
        transform.position = new Vector3(posX, posY, 0);
    }
}
