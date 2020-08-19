﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <include file='docs.xml' path='docs/members[@name="reset"]/ResetButtonBehaviour/*'/>
public class ResetButtonBehaviour : MonoBehaviour
{
    // only true when physics has been turned on since the last reset (or since the launching of the Contraption Designer, if there hasn't been a reset yet)
    // keep this private so that other classes have to use the setter, which also toggles button's interactable property
    private bool resettable;

    /// <include file='docs.xml' path='docs/members[@name="reset"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="reset"]/piecesScrollView/*'/>
    public GameObject piecesScrollView; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="reset"]/startStopObject/*'/>
    public GameObject startStopObject; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="reset"]/pieceControlsPanel/*'/>
    public GameObject pieceControlsPanel; // connected in editor
    
    private RaycastingBehaviour raycastingScript;
    
    private StartStopButtonBehaviour startStopButtonScript;
    
    // Start is called before the first frame update
    void Start()
    {
        resettable = false;
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
        startStopButtonScript = startStopObject.GetComponent<StartStopButtonBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="reset"]/OnResetButtonPress/*'/>
    public void OnResetButtonPress(){
        
        // ignore button press if a piece is moving or being placement-corrected
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }

        // reenable the start/stop button, in the event that it had been disabled temporarily through the temporary removal of the last piece
        startStopObject.GetComponent<Button>().interactable = true;

        // turn off physics, if it's on
        if(startStopButtonScript.physicsOn){
            startStopButtonScript.OnStartStopPress();
        }

        piecesScrollView.SetActive(true); // needs to be after the OnStartStopPress call    
        pieceControlsPanel.SetActive(true); // same
        
        // Reactivate the temporarily removed pieces and move them to the normal pieces list
        foreach (GameObject piece in raycastingScript.piecesRemovedWhileResettable){
            piece.SetActive(true);
            raycastingScript.pieces.Add(piece);
        }
        raycastingScript.piecesRemovedWhileResettable.Clear();

        // Before resetting *any* positions (which will cause OnTriggerEnter to fire for the pieces),
        // clear *all* of the collidersInContact lists so that we don't have unwanted duplicates
        // (so don't merge this loop with the next one!)
        foreach(GameObject piece in raycastingScript.pieces){
            PiecePrefabBehaviour pieceScript = piece.GetComponent<PiecePrefabBehaviour>();
            pieceScript.collidersInContact.Clear();
        }
        
        // Finally, actually do the resetting
        foreach (GameObject piece in raycastingScript.pieces){
            PiecePrefabBehaviour pieceScript = piece.GetComponent<PiecePrefabBehaviour>();
            pieceScript.resetTransforms();
            pieceScript.clearVelocities(); // needs to be after the OnStartStopPress call
        }

        setResettable(false); // needs to be after the OnStartStopPress call
    }

    /// <include file='docs.xml' path='docs/members[@name="reset"]/setResettable/*'/>
    public void setResettable(bool resettable){
        
        // set the button's interactable property
        gameObject.GetComponent<Button>().interactable = resettable;
        
        // set resettable instance variable
        this.resettable = resettable;
        
        // update the start/stop button appearance, if applicable
        if(!resettable){
            if(startStopButtonScript != null){
                startStopButtonScript.setButtonState("start");
            }
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="reset"]/getResettable/*'/>
    public bool getResettable(){
        return resettable;
    }
}