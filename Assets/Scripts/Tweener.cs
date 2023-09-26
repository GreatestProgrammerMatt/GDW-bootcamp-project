using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    public enum ETypeOfTween
    {
        PopUpGrow,
        PopUpGrowLoopGrow,
        PopUpGrowLoopRot,
        LoopingGrow,
        LoopingRotation,
        PopBounce,
        LoopingMiniRot,
        PopGrowLoopingMiniRot
    }

    public ETypeOfTween myTween;
    private Transform myOriginalTransform;
    public float magnitud;
    public float speed;
    public float lspeed;
    public LeanTweenType ease;
    public float delay;
    public int loops;


    private void OnEnable()
    {
        LeanTween.cancel(gameObject);
        myOriginalTransform = gameObject.transform;
        var seq = LeanTween.sequence();
        switch (myTween)
        {
            case ETypeOfTween.PopUpGrow:
                transform.localScale=new Vector3(0,0,0);
                LeanTween.scale(gameObject,new Vector3(1,1,1),speed).setDelay(delay).setEase(ease);
            break;
            case ETypeOfTween.PopUpGrowLoopGrow:
                transform.localScale=new Vector3(0,0,0);
                LeanTween.scale(gameObject,new Vector3(1,1,1),speed).setDelay(delay).setEase(ease).setOnComplete(
                    ()=> {LeanTween.scale(gameObject,new Vector3(magnitud*0.75f,magnitud*0.75f,magnitud*0.75f),speed*2).setDelay(delay).setEase(ease).setLoopPingPong(loops);}
                );
            break;
            case ETypeOfTween.PopUpGrowLoopRot:
                transform.localScale=new Vector3(0,0,0);
                LeanTween.scale(gameObject,new Vector3(1,1,1),speed).setDelay(delay).setEase(ease).setOnComplete(
                    ()=> {
                        LeanTween.rotateAround(gameObject,Vector3.forward,-magnitud,lspeed).setOnComplete(
                            ()=> {LeanTween.rotateAround(gameObject,Vector3.forward,magnitud*2,lspeed*2).setLoopPingPong(loops);}
                        );
                    }
                );
            break;
            case ETypeOfTween.LoopingGrow:
                transform.localScale=new Vector3(1,1,1);
                LeanTween.scale(gameObject,new Vector3(magnitud,magnitud,magnitud),speed).setDelay(delay).setEase(ease).setLoopPingPong(loops);
            break;
            case ETypeOfTween.LoopingRotation:
                LeanTween.rotateZ(gameObject,-magnitud,lspeed).setOnComplete(
                            ()=> {LeanTween.rotateZ(gameObject,magnitud*2,lspeed*2).setLoopPingPong(loops);}
                        );
            break;
            case ETypeOfTween.PopBounce:
                transform.localScale=new Vector3(0,0,0);
                LeanTween.scale(gameObject,new Vector3(magnitud,magnitud,magnitud),speed/2).setDelay(delay).setEase(ease).setOnComplete(
                    ()=> {LeanTween.scale(gameObject,new Vector3(1,1,1),speed/2).setDelay(delay).setEase(ease);}
                );
            break;
            case ETypeOfTween.LoopingMiniRot:
                LeanTween.rotateAroundLocal(gameObject,Vector3.forward,magnitud,lspeed).setDelay(delay).setLoopClamp();
            break;
            case ETypeOfTween.PopGrowLoopingMiniRot:
                transform.localScale=new Vector3(0,0,0);
                LeanTween.scale(gameObject,new Vector3(2,2,2),speed/2).setOnComplete(
                    ()=> {LeanTween.scale(gameObject,new Vector3(1,1,1),speed/2).setOnComplete(
                        ()=> {
                            LeanTween.rotateAroundLocal(gameObject,Vector3.forward,magnitud,lspeed).setLoopClamp();
                        }
                    );}
                );
            break;
        }    
    }
}
