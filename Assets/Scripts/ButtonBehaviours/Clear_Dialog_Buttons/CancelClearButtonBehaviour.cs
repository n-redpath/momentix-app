using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="cancelClear"]/CancelClearButtonBehaviour/*'/>
public class CancelClearButtonBehaviour : MonoBehaviour
{
    
    /// <include file='docs.xml' path='docs/members[@name="cancelClear"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor

    private RaycastingBehaviour raycastingScript;

    /// <include file='docs.xml' path='docs/members[@name="cancelClear"]/clearDialogPanel/*'/>
    public GameObject clearDialogPanel; // connected in editor
    
    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="cancelClear"]/OnCancelClearButtonPress/*'/>
    public void OnCancelClearButtonPress(){
        raycastingScript.clearDialogShowing = false;
        clearDialogPanel.SetActive(false);
    }
}
