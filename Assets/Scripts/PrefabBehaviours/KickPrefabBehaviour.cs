using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="kick"]/KickPrefabBehaviour/*'/>
public class KickPrefabBehaviour : PiecePrefabBehaviour
{
    private GameObject topRenderedMediumArmObject;
    private GameObject firstRenderedBottomMediumArmObject;

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/pieceSpecificSetup/*'/>
    protected override void pieceSpecificSetup(){
        pieceDisplayName = "Kick";
        snapToLayer = 13;
        topRenderedMediumArmObject = transform.Find("Rotating Arms and Pegs").Find("Medium Arm v18:2").Find("Medium Arm v18 1").Find("Body1 1").gameObject;
        firstRenderedBottomMediumArmObject = transform.Find("Supported Pulley v7:1").Find("Supported Pulley v7").Find("Support Structure v11:1").Find("Support Structure v11").Find("Medium Arm v18:1 1").Find("Medium Arm v18 2").Find("Body1 10").gameObject;
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
        // recursively toggle the isKinematic property on all descendents with a Rigidbody
        processParentKinematic(gameObject, kinematic);
    }

    // recursive helper function that toggles isKinematic for the given GameObject (parent), if it has a Rigidbody, and then calls itself on each child of that object
    private void processParentKinematic(GameObject parent, bool kinematic){
        Rigidbody rb = parent.GetComponent<Rigidbody>();
        if(rb != null){
            rb.isKinematic = kinematic;
        }
        foreach(Transform child_trans in parent.transform){
            processParentKinematic(child_trans.gameObject, kinematic);
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/setTriggers/*'/>
    public override void setTriggers(bool triggers){
        // recursively toggle the isTrigger property on all descendents with a Collider
        processParentTrigger(gameObject, triggers);
    }

    // recursive helper function that toggles isTrigger for the given GameObject (parent), if it has a Collider, and then calls itself on each child of that object
    private void processParentTrigger(GameObject parent, bool triggers){
        Collider col = parent.GetComponent<Collider>();
        if(col != null && parent.layer == 0){ // only toggle isTrigger for the default layer
            col.isTrigger = triggers;
        }
        foreach(Transform child_trans in parent.transform){
            processParentTrigger(child_trans.gameObject, triggers);
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getHalo/*'/>
    public override Behaviour getHalo(){
        return transform.Find("Supported Pulley v7:1").Find("Supported Pulley v7").Find("Support Structure v11:1").Find("Halo and Box Collider").gameObject.GetComponent("Halo") as Behaviour;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getHeight/*'/>
    protected override float getHeight(){
        return getTop() - getBottom();
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getTop/*'/>
    protected override float getTop(){
        return topRenderedMediumArmObject.transform.position.y + topRenderedMediumArmObject.GetComponent<MeshRenderer>().bounds.size.y;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/getBottom/*'/>
    protected override float getBottom(){
        return firstRenderedBottomMediumArmObject.transform.position.y - firstRenderedBottomMediumArmObject.GetComponent<MeshRenderer>().bounds.size.y;
    }

    /// <include file='docs.xml' path='docs/members[@name="piecePrefab"]/convertBottomToTransformY/*'/>
    protected override float convertBottomToTransformY(float bottom){
        return bottom + transform.position.y - getBottom();
    }
}
