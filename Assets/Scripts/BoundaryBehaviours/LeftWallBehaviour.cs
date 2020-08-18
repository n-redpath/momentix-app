using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="leftWall"]/LeftWallBehaviour/*'/>
public class LeftWallBehaviour : MonoBehaviour
{

    /// <include file='docs.xml' path='docs/members[@name="leftWall"]/mainScriptObject/*'/>
    public GameObject mainScriptObject; // connected in editor
    private RaycastingBehaviour raycastingScript;

    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = mainScriptObject.GetComponent<RaycastingBehaviour>();
    }

    /// <include file='docs.xml' path='docs/members[@name="leftWall"]/OnTriggerEnter/*'/>
    private void OnTriggerEnter(Collider other){
        Transform parent = other.gameObject.transform.parent;
        if(parent == null || parent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece causing the collision
            GetCompletePiece(other.gameObject).GetComponent<PiecePrefabBehaviour>().canMoveTowardsNegX = false;
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="leftWall"]/OnTriggerExit/*'/>
    private void OnTriggerExit(Collider other){
        Transform parent = other.gameObject.transform.parent;
        if(parent == null || parent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece that had caused the collision
            GetCompletePiece(other.gameObject).GetComponent<PiecePrefabBehaviour>().canMoveTowardsNegX = true;
        }
    }

    // this recursive function works up the hierarchy looking for an object with a PiecePrefabBehaviour (implemented because of compound colliders)
    /// <include file='docs.xml' path='docs/members[@name="leftWall"]/GetCompletePiece/*'/>
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
