using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /*private Image JoystickBG;
    private Image touch;
    [HideInInspector]
    public Vector3 InputDir;
    
    public bool Shoot=false;
    private void Start() 
    {
        JoystickBG=GetComponent<Image>();
        touch=transform.GetChild(0).GetComponent<Image>();
        InputDir=Vector3.zero;  
    }
    public void OnDrag(PointerEventData eventData)
    {
       Vector2 position=Vector2.zero;
       if(RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickBG.rectTransform,
                 eventData.position, eventData.pressEventCamera, out position))
                 {
                     position.x=(position.x/JoystickBG.rectTransform.sizeDelta.x);
                     position.y=(position.y/JoystickBG.rectTransform.sizeDelta.y);

                     float x=(JoystickBG.rectTransform.pivot.x==1f)? position.x*2 + 1:position.x*2 - 1;
                     float y=(JoystickBG.rectTransform.pivot.y==1f)? position.y*2 + 1:position.y*2 - 1;

                     InputDir=new Vector3(x,y,0);
                     InputDir=(InputDir.magnitude>1)? InputDir.normalized:InputDir;

                     touch.rectTransform.anchoredPosition=new Vector3(InputDir.x*(JoystickBG.rectTransform.sizeDelta.x/2.5f),
                                                                    InputDir.y*(JoystickBG.rectTransform.sizeDelta.y/2.5f));
                 if(!Shoot)
                 FindObjectOfType<Spaceship>().CanShoot=false;
                 if(Shoot)
                 FindObjectOfType<Spaceship>().CanShoot=true;
                 }
                 
                 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!Shoot)
        InputDir=Vector3.zero;
        else
        FindObjectOfType<Spaceship>().CanShoot=false;
        touch.rectTransform.anchoredPosition=Vector3.zero;
    }
    */
}
