using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <include file='docs.xml' path='docs/members[@name="startStop"]/StartStopButtonBehaviour/*'/>
public class StartStopButtonBehaviour : MonoBehaviour
{

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/pieceControlsPanel/*'/>
    public GameObject pieceControlsPanel; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/piecesScrollView/*'/>
    public GameObject piecesScrollView; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/playImg/*'/>
    public Sprite playImg; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/pauseImg/*'/>
    public Sprite pauseImg; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/clearAllObject/*'/>
    public GameObject clearAllObject; // connected in editor

    private Button clearAllButton;

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/workspaceBoundariesObject/*'/>
    public GameObject workspaceBoundariesObject; // connected in editor

    private RaycastingBehaviour raycastingScript;

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/physicsOn/*'/>
    public bool physicsOn;

    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
        clearAllButton = clearAllObject.GetComponent<Button>();
        physicsOn = false;
    }

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/OnStartStopPress/*'/>
    public void OnStartStopPress(){
        
        // ignore the button press if a piece is moving or being placement-corrected
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }
        
        // toggle physicsOn instance variable
        physicsOn = !physicsOn;
        
        // handle the saving/resuming of positions, rotations, and velocities
        foreach (GameObject piece in raycastingScript.pieces){
            PiecePrefabBehaviour pieceScript = piece.GetComponent<PiecePrefabBehaviour>();
            if(physicsOn && !raycastingScript.resetButtonScript.getResettable()){
                // If physics is on, save positions and rotations, if they haven't already been saved since the last reset (or since the Contraption Designer loaded, if there hasn't been a reset yet)
                // i.e. this code runs when the user pressed a button that said, "start"
                pieceScript.saveTransforms();
            }else if(physicsOn){
                // Here, physics is being turned on and has already been turned on before since the last reset, so resume those velocities
                // i.e. this code runs when the user pressed a button that said, "resume"
                pieceScript.resumeVelocities();
            }else{
                // Here, physics is being turned off, so save the velocities in case the user wants to resume subsequently
                // i.e. this code runs when the user pressed a button that said, "pause"
                pieceScript.saveVelocities();
            }
        }

        // set the state of the reset button and corresponding data
        raycastingScript.resetButtonScript.setResettable(true);

        // set the appearance of this button, and make sure to clear the active piece variable
        if(physicsOn){
            raycastingScript.ClearActivePiece(); // probably not necessary since pressing the button should clear active piece anyway, but might as well play it safe
            setButtonState("pause"); // change the text and color of the button
        }else{
            setButtonState("resume"); // change the text and color of the button
        }
        
        // disable piece control buttons (even when turning physics off, since we still don't have an active piece yet at that point. But it's all irrelevant anyway if we don't allow paused-machine editing)
        raycastingScript.SetAllPieceControlsButtonsInteractable(false);

        // toggle the interactability of elements that are only interactable when physics is off
        clearAllButton.interactable = !physicsOn;

        // turn off visibility of pieces scroll view and piece controls panel, since we don't want to allow user to add or edit pieces before resetting
        piecesScrollView.SetActive(false);
        pieceControlsPanel.SetActive(false);
        
        // toggle isKinematic and isTrigger for all pieces
        foreach(GameObject piece in raycastingScript.pieces){
            PiecePrefabBehaviour pieceScript = piece.GetComponent<PiecePrefabBehaviour>();
            pieceScript.setKinematic(!physicsOn);
            pieceScript.setTriggers(!physicsOn); // needs to be after the ClearActivePiece call
        }

        // toggle isTrigger for all workspace boundary colliders 
        foreach(Transform bound_trans in workspaceBoundariesObject.transform){
            GameObject boundary = bound_trans.gameObject;
            boundary.GetComponent<Collider>().isTrigger = !physicsOn;
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="startStop"]/setButtonState/*'/>
    public void setButtonState(string state){
        Text button_text = GetComponentInChildren<Text>();
        switch(state){
            case "pause":
                GetComponent<Image>().sprite = pauseImg;
                break;
            case "start":
                // currently just letting this fall through into the "resume" functionality, since there's no difference between the two states
            case "resume":
                GetComponent<Image>().sprite = playImg;
                break;
            default:
                break;
        }
    }
}
