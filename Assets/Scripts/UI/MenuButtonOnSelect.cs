using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButtonOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] TMP_Text text;
    

    // This works for controller
    public void OnSelect(BaseEventData eventData) {
         changeTextSelected();
    }
    
    public void OnDeselect(BaseEventData eventData) {
        changeTextUnselected();
    }

    // This works for keyboard & mouse
    public void OnPointerEnter(PointerEventData eventData) {
        changeTextSelected();      
        
    }

    public void OnPointerExit(PointerEventData eventData) {
        changeTextUnselected();
    }

    private void changeTextUnselected() {
        text.enableVertexGradient = true;
    }

    private void changeTextSelected() {
        text.enableVertexGradient = false;
    }

}
