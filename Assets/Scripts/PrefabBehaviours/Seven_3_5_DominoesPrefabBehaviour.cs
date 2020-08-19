using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <include file='docs.xml' path='docs/members[@name="seven35Dominoes"]/Seven_3_5_DominoesPrefabBehaviour/*'/>
public class Seven_3_5_DominoesPrefabBehaviour : PiecePrefabBehaviour
{

    private GameObject oneDominoRenderedObject;

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/pieceSpecificSetup/*'/>
    protected override void pieceSpecificSetup(){
        pieceDisplayName = "Dominoes";
        snapToLayer = 16;
        oneDominoRenderedObject = transform.Find("One_Domino").Find("group_0_16777215").gameObject;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/movePiece/*'/>
    protected override void movePiece(Vector2 touchPosition){
        
        // calculate new world-space position for piece
        Vector2 screenTranslation = touchPosition - prevFrameTouchPosition;
        Vector3 currScreenPosition = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 newScreenPosition = new Vector3(currScreenPosition.x + screenTranslation.x, currScreenPosition.y + screenTranslation.y, currScreenPosition.z);
        Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(newScreenPosition);
        
        // prevent the piece from moving in a direction that isn't currently allowed for it (i.e. from moving beyond a workspace boundary)
        if(!canMoveDown && newWorldPosition.y < transform.position.y){
            newWorldPosition.y = transform.position.y;
        }
        if(!canMoveTowardsNegX && newWorldPosition.x < transform.position.x){
            newWorldPosition.x = transform.position.x;
        }
        if(!canMoveTowardsPosX && newWorldPosition.x > transform.position.x){
            newWorldPosition.x = transform.position.x;
        }
        if(!canMoveTowardsNegZ && newWorldPosition.z < transform.position.z){
            newWorldPosition.z = transform.position.z;
        }
        if(!canMoveTowardsPosZ && newWorldPosition.z > transform.position.z){
            newWorldPosition.z = transform.position.z;
        }

        // move the piece
        transform.position = newWorldPosition;

        // update instance variable so it's ready for the next time the finger moves
        prevFrameTouchPosition = touchPosition;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/setKinematic/*'/>
    public override void setKinematic(bool kinematic){
        // This logic assumes the only rigidbodies are on the direct children of the root prefab gameobject
        foreach(Transform child_trans in transform){
            child_trans.gameObject.GetComponent<Rigidbody>().isKinematic = kinematic;
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/setTriggers/*'/>
    public override void setTriggers(bool triggers){
        // This logic assumes that the only colliders we're interested in changing are on the direct children of the root prefab gameobject
        foreach(Transform child_trans in transform){
            child_trans.gameObject.GetComponent<Collider>().isTrigger = triggers;
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getHalo/*'/>
    public override Behaviour getHalo(){
        return GetComponent("Halo") as Behaviour;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getHeight/*'/>
    protected override float getHeight(){
        return oneDominoRenderedObject.GetComponent<MeshRenderer>().bounds.size.y;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getTop/*'/>
    protected override float getTop(){
        return transform.position.y + getHeight() / 2;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getBottom/*'/>
    protected override float getBottom(){
        return transform.position.y - getHeight() / 2;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/convertBottomToTransformY/*'/>
    protected override float convertBottomToTransformY(float bottom){
        return bottom + getHeight() / 2;
    }
}
