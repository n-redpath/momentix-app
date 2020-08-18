using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="clearAll"]/ClearAllButtonBehaviour/*'/>
public class ClearAllButtonBehaviour : MonoBehaviour
{

    /// <include file='docs.xml' path='docs/members[@name="clearAll"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    RaycastingBehaviour raycastingScript;

    /// <include file='docs.xml' path='docs/members[@name="clearAll"]/clearDialogPanel/*'/>
    public GameObject clearDialogPanel; // connected in editor
    
    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="clearAll"]/OnClearAllButtonPress/*'/>
    public void OnClearAllButtonPress(){
        
        // ignore the button press if a piece is being moved or placement-corrected
        if(raycastingScript.activePiece != null){
            PiecePrefabBehaviour activePieceBehaviour = raycastingScript.activePiece.GetComponent<PiecePrefabBehaviour>();
            if(activePieceBehaviour.isMoving() || activePieceBehaviour.isPlacementCorrecting()){
                return;
            }
        }
        
        raycastingScript.clearDialogShowing = true;
        clearDialogPanel.SetActive(true);
    }
}
