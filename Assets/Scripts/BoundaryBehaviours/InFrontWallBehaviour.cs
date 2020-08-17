using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/InFrontWallBehaviour/*'/>
public class InFrontWallBehaviour : MonoBehaviour
{

    /// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor
    private RaycastingBehaviour raycastingScript;

    // Start is called before the first frame update
    /// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/Start/*'/>
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/OnTriggerEnter/*'/>
    private void OnTriggerEnter(Collider other){
        Transform parent = other.gameObject.transform.parent;
        if(parent == null || parent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece causing the collision
            GetCompletePiece(other.gameObject).GetComponent<PiecePrefabBehaviour>().canMoveTowardsPosZ = false;
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/OnTriggerExit/*'/>
    private void OnTriggerExit(Collider other){
        Transform parent = other.gameObject.transform.parent;
        if(parent == null || parent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece that had caused the collision
            GetCompletePiece(other.gameObject).GetComponent<PiecePrefabBehaviour>().canMoveTowardsPosZ = true;
        }
    }

    // this recursive function works up the hierarchy looking for an object with a PiecePrefabBehaviour (implemented because of compound colliders)
    /// <include file='docs.xml' path='docs/members[@name="inFrontWall"]/GetCompletePiece/*'/>
    private GameObject GetCompletePiece(GameObject child){
        if(child.GetComponent<PiecePrefabBehaviour>() != null){ 
            return child;
        }
        if(child.transform.parent == null){
            return null;
        }
        return GetCompletePiece(child.transform.parent.gameObject);
    }
}
