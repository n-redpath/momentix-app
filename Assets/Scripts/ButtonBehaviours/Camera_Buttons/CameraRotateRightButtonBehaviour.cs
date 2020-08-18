using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <include file='docs.xml' path='docs/members[@name="rotateRight"]/CameraRotateRightButtonBehaviour/*'/>
public class CameraRotateRightButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    private int camRotationDirection; // 0 is not moving, 1 moves left, -1 moves right

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/DEGREES_PER_SECOND/*'/>
    public const float DEGREES_PER_SECOND = 45;

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    private RaycastingBehaviour raycastingScript;
    
    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/camRotationCenter/*'/>
    public GameObject camRotationCenter; // connected in editor

    // Start is called before the first frame update
    void Start()
    {
        camRotationDirection = 0;
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.gameObject.transform.RotateAround(camRotationCenter.transform.position, raycastingScript.camAxisOfHorizRotation, camRotationDirection * DEGREES_PER_SECOND * Time.deltaTime);
    }

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/OnPointerDown/*'/>
    public void OnPointerDown(PointerEventData data){
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }
        camRotationDirection = -1;
    }
    
    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/OnPointerUp/*'/>
    public void OnPointerUp(PointerEventData data){
        camRotationDirection = 0;
    }

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/rotating/*'/>
    public bool rotating(){
        return camRotationDirection != 0;
    }
}
