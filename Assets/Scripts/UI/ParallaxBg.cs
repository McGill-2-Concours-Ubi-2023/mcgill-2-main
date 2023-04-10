using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ParallaxBg : MonoBehaviour
{
    Vector2 StartPos; 

    [SerializeField] int moveModifier;
    [SerializeField] float modifier2;
    [SerializeField] float controllerModifier;
    [SerializeField] GameObject positionDecider;

    ParallaxPosition pos;

    private void Start() {
        StartPos = transform.position;
        pos = positionDecider.GetComponent<ParallaxPosition>();
    }

    private void Update() {
        Vector2 position =  pos.position* modifier2;

        Vector2 controllerPos = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (controllerPos.x != 0 || controllerPos.y != 0) {
            position = controllerPos * controllerModifier;
            position.y = position.y/4;
        }

        float posX = Mathf.Lerp(transform.position.x, StartPos.x + (position.x * moveModifier), 2f * Time.deltaTime);
        float posY = Mathf.Lerp(transform.position.y, StartPos.y + (position.y * moveModifier), 2f * Time.deltaTime);
        
        transform.position = new Vector3(posX, posY, 0);
    }
}
