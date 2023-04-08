using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParallaxBg : MonoBehaviour
{
    Vector2 StartPos; 

    [SerializeField] int moveModifier;
    [SerializeField] float modifier2;

    private void Start() {
        StartPos = transform.position;
    }

    private void Update() {

        Vector2 controllerPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        float posX = Mathf.Lerp(transform.position.x, StartPos.x + (controllerPos.x * moveModifier * modifier2), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, StartPos.y + (controllerPos.y/3 * moveModifier * modifier2), 2f * Time.deltaTime);
        
        transform.position = new Vector3(posX, posY, 0);
    }
}
