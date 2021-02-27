using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public static class UIAnimation
{
    // Start is called before the first frame update
    public static bool isAnimating = false;
    /// <summary>
    /// Animates the screen to go Out in Vertical, then it deactivates the GameObject
    /// </summary>
    /// <param name="screen">The Screen to hide</param>
    public static void HideVertical(RectTransform screen){

        isAnimating = true;
        screen.DOAnchorPosY(1500,0.7f,true).SetEase(Ease.InBack).OnComplete(()=>{
            screen.gameObject.SetActive(false);
            screen.anchoredPosition = new Vector2(0f,0f);
            isAnimating = false;
        });
    }

    /// <summary>
    /// Animates the screen to appear in Vertical, it activates the GameObject before the animation
    /// </summary>
    /// <param name="screen">The Screen To Show</param>
    public static void ShowVertical(RectTransform screen){
            
        isAnimating = true;
        screen.gameObject.SetActive(true);
        screen.DOAnchorPosY(1500,0.7f,true).SetEase(Ease.OutBack).From().OnComplete(()=>{
            isAnimating = false;
        });
    }

    public static void ShowHorizontal(RectTransform screen,float time){
        if(screen.GetComponent<CanvasGroup>())
            screen.GetComponent<CanvasGroup>().alpha = 1;
        isAnimating = true;
        screen.gameObject.SetActive(true);
        screen.transform.SetAsLastSibling();
        screen.DOAnchorPosX(screen.rect.width,0.7f,true).SetEase(Ease.OutCubic).From().OnComplete(()=>{
            isAnimating = false;
        });
    }

    public static void ShowHorizontal(RectTransform screen){
        ShowHorizontal(screen,0.7f);
    }

    public static void HideHorizontal(RectTransform screen){
        isAnimating = true;
        screen.gameObject.SetActive(true);
        screen.transform.SetAsLastSibling();
        screen.DOAnchorPosX(screen.rect.width,0.7f,true).SetEase(Ease.OutCubic).OnComplete(()=>{
            isAnimating = false;
            screen.anchoredPosition = new Vector2(0f,0f);
            screen.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// It deactivates and disappear a button
    /// </summary>
    /// <param name="button">The Button to hide</param>
    public static void FadeOutButton(Button button){
        button.interactable = false;
        button.GetComponent<Image>().DOFade(0f,0.5f);
    }

    /// <summary>
    /// Activates and shows a button
    /// </summary>
    /// <param name="button">The button to show</param>
    public static void FadeInButton(Button button){
        button.GetComponent<Image>().DOFade(1f,0.5f).OnComplete(()=>{
            button.interactable = true;
        });
    }

    /// <summary>
    /// Fades Out a Screen then it deactivates the GameObject
    /// </summary>
    /// <param name="screen">The Screen to fade out</param>
    /// <param name="animationTime">The time applied to fade out the screen</param>
    public static void FadeOutScreen(CanvasGroup screen,float animationTime){
        FadeOutScreen(screen,animationTime,0f);
    }

    public static void FadeOutScreen(CanvasGroup screen,float animationTime,float delayTime, bool keepActive){
        isAnimating = true;
        screen.DOFade(0f,animationTime).SetDelay(delayTime).OnComplete(()=>{
            screen.gameObject.SetActive(keepActive);
            isAnimating = false;
        });
    }

    public static void FadeOutScreen(CanvasGroup screen,float animationTime,float delayTime){
        isAnimating = true;
        screen.DOFade(0f,animationTime).SetDelay(delayTime).OnComplete(()=>{
            screen.gameObject.SetActive(false);
            isAnimating = false;
        });
    }

    /// <summary>
    /// Fades In a screen, it activates the GameObject first.
    /// </summary>
    /// <param name="screen">The Screen to fade in</param>
    /// <param name="animationTime">The time applied to fade in the screen</param>
    /// <param name="delayTime">The time used to delay the animation</param>
    public static void FadeInScreen(CanvasGroup screen,float animationTime, float delayTime){
        screen.gameObject.SetActive(true);
        screen.alpha = 0;
        screen.DOFade(1f,animationTime).SetDelay(delayTime).OnComplete(()=>{
            isAnimating = false;
        });
    }

    public static void FadeInScreen(CanvasGroup screen,float animationTime){
        FadeInScreen(screen,animationTime,0);
    }

    public static void ScaleOut(Transform transform,float scaleToUse, float timeToUse){
        transform.DOScale(scaleToUse,timeToUse).SetEase(Ease.InQuad);
    }

    /// <summary>
    /// Animates a button, then it runs a callback(UnityEvent)
    /// </summary>
    /// <param name="transform">The button to animate</param>
    /// <param name="buttonCallback">The callback to run after the animation</param>
    public static void AnimateButton(Transform transform,UnityEvent buttonCallback,int animationID){
        
        isAnimating = true;
        if(transform.GetComponent<Animator>() != null)
            transform.GetComponent<Animator>().enabled = false;

        var scaleToUse = transform.localScale;
        switch(animationID){
            case 0:
                transform.DOScale(new Vector3(scaleToUse.x * 0.75f,scaleToUse.y * 0.75f,scaleToUse.z * 0.75f),0.15f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{
                    isAnimating = false;
                    buttonCallback.Invoke();
                });
            break;
            case 1:
                transform.DOScale(new Vector3(scaleToUse.x * 0.65f,scaleToUse.y * 0.65f,scaleToUse.z * 0.65f),0.15f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{
                transform.DOScale(new Vector3(scaleToUse.x * 0.8f,scaleToUse.y * 0.8f,scaleToUse.z * 0.8f),0.07f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{
                        isAnimating = false;
                        buttonCallback.Invoke();
                    });
                });
            break;
            case 2:
                isAnimating = false;
                buttonCallback.Invoke();
            break;
        }
    }

    public static void SquishObject(Transform transform){

        var scaleToUse = transform.localScale;
        transform.DOScale(new Vector3(scaleToUse.x * 0.65f,scaleToUse.y * 0.65f,scaleToUse.z * 0.65f),0.15f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{
            transform.DOScale(new Vector3(scaleToUse.x * 0.8f,scaleToUse.y * 0.8f,scaleToUse.z * 0.8f),0.07f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{
            });
        });
    }

    public static void UpDownObject(Transform transform,float distance){

        float yPosition = transform.position.y;
        transform.DOMoveY(yPosition + distance,0.25f,false).SetLoops(2,LoopType.Yoyo).OnComplete(()=>{

        });
    }

    public static void ThrowObject(Transform transform, Transform objective,UnityEvent callback){

        
        transform.DOMove(objective.position,1f,false).OnComplete(()=>{
            callback.Invoke();
        });
        //transform.DOMoveY(objective.position.y + 2,0.5f,false).SetLoops(2,LoopType.Yoyo);
    }

    public static void PopImage(Transform transform, float time,float delay){
        transform.gameObject.SetActive(true);
        Vector3 scaleToUse = transform.localScale;
        transform.DOScale(new Vector3(0f,0f,0f),time).From().OnComplete(()=>{
            transform.DOScale(new Vector3(scaleToUse.x * 0.8f,scaleToUse.y * 0.8f,scaleToUse.z * 0.8f),time * 0.45f).SetDelay(delay).SetLoops(2,LoopType.Yoyo);
        });
    }

    public static void PopImage(Transform transform, float time){
        PopImage(transform,time,0f);
    }
}
