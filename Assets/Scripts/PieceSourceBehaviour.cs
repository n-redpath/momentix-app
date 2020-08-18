using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <include file='docs.xml' path='docs/members[@name="pieceSource"]/PieceSourceBehaviour/*'/>
public class PieceSourceBehaviour : MonoBehaviour, IPointerDownHandler
{
    
    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/piecePrefab/*'/>
    public GameObject piecePrefab; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    private RaycastingBehaviour raycastingScript;
    
    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/camLeftButton/*'/>
    public GameObject camLeftButton; // connected in editor
    
    private CameraRotateLeftButtonBehaviour camLeftButtonScript;
    
    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/camRightButton/*'/>
    public GameObject camRightButton; // connected in editor
    
    private CameraRotateRightButtonBehaviour camRightButtonScript;
    
    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/pieceLeftButton/*'/>
    public GameObject pieceLeftButton; // connected in editor
    
    private PieceRotateLeftButtonBehaviour pieceLeftButtonScript;
    
    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/pieceRightButton/*'/>
    public GameObject pieceRightButton; // connected in editor
    
    private PieceRotateRightButtonBehaviour pieceRightButtonScript;
    
    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
        camLeftButtonScript = camLeftButton.GetComponent<CameraRotateLeftButtonBehaviour>();
        camRightButtonScript = camRightButton.GetComponent<CameraRotateRightButtonBehaviour>();
        pieceLeftButtonScript = pieceLeftButton.GetComponent<PieceRotateLeftButtonBehaviour>();
        pieceRightButtonScript = pieceRightButton.GetComponent<PieceRotateRightButtonBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="pieceSource"]/OnPointerDown/*'/>
    public void OnPointerDown(PointerEventData data){
        
        // ignore touch if physics simulation is on
        if(raycastingScript.startStopButtonScript.physicsOn){
            return;
        }

        // ignore touch if a piece is currently moving or being placement-corrected
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }

        // ignore touch if user is currently rotating the camera or rotating a piece
        if(camLeftButtonScript.rotating() || camRightButtonScript.rotating() || pieceLeftButtonScript.rotating() || pieceRightButtonScript.rotating()){
            return;
        }

        // set resettable to false since the new piece doesn't have a place in the previous saved state
        // (although this command isn't needed if the user isn't allowed to add pieces while machine is resettable anyway)
        raycastingScript.resetButtonScript.setResettable(false);

        // instantiate a new piece and run some setup for it
        GameObject newPiece = Instantiate(piecePrefab);
        PiecePrefabBehaviour newPieceScript = newPiece.GetComponent<PiecePrefabBehaviour>();
        newPieceScript.prefab = piecePrefab;
        newPieceScript.OnPieceTouchBegin();
    }

}
