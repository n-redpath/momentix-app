using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <include file='docs.xml' path='docs/members[@name="rotateRight"]/PieceRotateRightButtonBehaviour/*'/>
public class PieceRotateRightButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/DEGREES_PER_SECOND/*'/>
    public const float DEGREES_PER_SECOND = 45;
    
    private int pieceRotationDirection; // 0 is not moving, 1 moves left, -1 moves right

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    private RaycastingBehaviour raycastingScript;

    // Start is called before the first frame update
    void Start()
    {
        pieceRotationDirection = 0;
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if(raycastingScript.activePiece != null){
            raycastingScript.activePiece.transform.Rotate(0, pieceRotationDirection * DEGREES_PER_SECOND * Time.deltaTime, 0);
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/OnPointerDown/*'/>
    public void OnPointerDown(PointerEventData data){
        
        // ignore button press if a piece is currently moving or being placement-corrected
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }
        pieceRotationDirection = -1;
    }
    
    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/OnPointerUp/*'/>
    public void OnPointerUp(PointerEventData data){
        pieceRotationDirection = 0;
    }

    /// <include file='docs.xml' path='docs/members[@name="rotateRight"]/rotating/*'/>
    public bool rotating(){
        return pieceRotationDirection != 0;
    }
}
