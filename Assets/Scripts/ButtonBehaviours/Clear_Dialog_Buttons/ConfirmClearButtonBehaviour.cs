using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="confirmClear"]/ConfirmClearButtonBehaviour/*'/>
public class ConfirmClearButtonBehaviour : MonoBehaviour
{

    /// <include file='docs.xml' path='docs/members[@name="confirmClear"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    private RaycastingBehaviour raycastingScript;

    /// <include file='docs.xml' path='docs/members[@name="confirmClear"]/resetObject/*'/>
    public GameObject resetObject; // connected in editor

    private ResetButtonBehaviour resetButtonScript; 
    
    /// <include file='docs.xml' path='docs/members[@name="confirmClear"]/clearDialogPanel/*'/>
    public GameObject clearDialogPanel; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="confirmClear"]/pieceControlsPanel/*'/>
    public GameObject pieceControlsPanel; // connected in editor
    
    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
        resetButtonScript = resetObject.GetComponent<ResetButtonBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="confirmClear"]/OnConfirmClearButtonPress/*'/>
    public void OnConfirmClearButtonPress(){
        foreach (GameObject piece in raycastingScript.pieces){
            Destroy(piece);
        }
        foreach (GameObject piece in raycastingScript.piecesRemovedWhileResettable){
            Destroy(piece); // since they were merely set to be inactive, not actually destroyed, before
        }
        raycastingScript.pieces.Clear();
        raycastingScript.piecesRemovedWhileResettable.Clear();
        resetButtonScript.setResettable(false);

        // redisplay piece controls panel
        pieceControlsPanel.SetActive(true);

        // put this after the list is cleared so it knows to disable the start button, etc.
        // put it after the piece controls panel is displayed so it knows to disable those buttons
        // also put it after resettable is set to false so it knows to redisplay the scroll view, etc.
        raycastingScript.ClearActivePiece(); 

        // undisplay the dialog and resume raycasting detection
        raycastingScript.clearDialogShowing = false;
        clearDialogPanel.SetActive(false);
    }
}
