using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public bool state = true;
    GameObject checkIcon;
    void Start()
    {

        checkIcon = transform.GetChild(0).gameObject;

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        state = !state;
        
        if (state)
        {
            checkIcon.SetActive(true);

        }
        else
        {
            checkIcon.SetActive(false);
        }
    }
    public void SetFalse()
    {
        state = false;
        checkIcon.SetActive(false);
    }
    public void SetTrue()
    {
        state = true;
        checkIcon.SetActive(true);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {

    }
}



    