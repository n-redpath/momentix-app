using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="pieceTrigger"]/PieceTriggerBehaviour/*'/>
public class PieceTriggerBehaviour : MonoBehaviour
{
    
    private RaycastingBehaviour raycastingScript;
    
    // Start is called before the first frame update
    void Start()
    {
        raycastingScript = GameObject.Find("Main Script Object").GetComponent<RaycastingBehaviour>();
    }

    private void OnTriggerEnter(Collider other){
        Transform otherParent = other.gameObject.transform.parent;
        if(otherParent == null || otherParent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece that had caused the collision
            GameObject collidingPiece = getCompletePiece(other.gameObject);
            GameObject thisPiece = getCompletePiece(gameObject);
            if(collidingPiece != thisPiece){ // make sure we don't process collisions that are betwen two colliders of the same piece--this happens with flat levers, for example    
                // add this GameObject's own collider to the incoming (colliding) GameObject's list of colliders in contact
                PiecePrefabBehaviour collidingPieceScript = collidingPiece.GetComponent<PiecePrefabBehaviour>();
                collidingPieceScript.collidersInContact.Add(gameObject.GetComponent<Collider>());
            }
        }
    }

    private void OnTriggerExit(Collider other){
        Transform parent = other.gameObject.transform.parent;
        if(parent == null || parent.gameObject.name != "Workspace Boundaries"){ // presumably, therefore, it's a machine piece that had caused the collision
            // remove this GameObject's own collider from the outgoing (de-colliding) GameObject's list of colliders in contact
            PiecePrefabBehaviour collidingPieceScript = getCompletePiece(other.gameObject).GetComponent<PiecePrefabBehaviour>();
            collidingPieceScript.collidersInContact.Remove(gameObject.GetComponent<Collider>());
        }
    }

    // this recursive function works up the hierarchy looking for an object with a PiecePrefabBehaviour (implemented because of compound colliders)
    private GameObject getCompletePiece(GameObject child){
        if(child.GetComponent<PiecePrefabBehaviour>() != null){ 
            return child;
        }
        if(child.transform.parent == null){
            return null;
        }
        return getCompletePiece(child.transform.parent.gameObject);
    }
}
