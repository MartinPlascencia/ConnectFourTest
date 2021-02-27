using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//[RequireComponent(typeof(Button))]
public class ButtonManager : MonoBehaviour, IPointerClickHandler
{
    //private Button button;
    public bool isPrincipal;
    public bool interactable = true;
    public enum AnimationType {ScaleDown,Squish,NoAnimation};
    public AnimationType animType;
    public UnityEvent buttonCallback;    

    public void OnPointerClick(PointerEventData eventData){
        if(UIAnimation.isAnimating || !interactable)
            return;
        //SoundManager.instance.Play("click");
        //Debug.Log(UIAnimation.isAnimating + " " + interactable);
        UIAnimation.AnimateButton(transform,buttonCallback,GetAnimationID());
    }

    int GetAnimationID(){
        int id = 0;
        switch(animType){
            case AnimationType.ScaleDown:
                id = 0;
            break;
            case AnimationType.Squish:
                id = 1;
            break;
            case AnimationType.NoAnimation:
                id = 2;
            break;
        }
        return id;
    }

    void OnEnable(){
        if(GetComponent<Animator>() != null)
            GetComponent<Animator>().enabled = true;
    }
}
