using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <include file='docs.xml' path='docs/members[@name="raycasting"]/RaycastingBehaviour/*'/>
public class RaycastingBehaviour : MonoBehaviour
{
    
    // instance variables regarding the camera, and the positions of the user's fingers that control its movement
    private Camera mainCamera;
    private Vector2 prevFrameFirstTouchPosition;
    private Vector2 prevFrameSecondTouchPosition;

    enum TouchAction {
        None, 
        MovingPiece,
        RotatingCamera,
        ZoomingAndPanningCamera
    }

    // stores the type of action the user is currently taking
    private TouchAction currTouchAction;
    
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/camRotationCenter/*'/>
    public GameObject camRotationCenter; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/camAxisOfHorizRotation/*'/>
    public Vector3 camAxisOfHorizRotation;
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/camAxisOfVertRotation/*'/>
    public Vector3 camAxisOfVertRotation;
    private float rotationDegreesPerPixel; // how many degrees the camera rotates when user's finger moves one pixel across the screen

    private float zoomDistPerPixel; // how far forward the camera moves when one of the user's fingers moves one pixel further on the screen from their other finger

    private float panDistPerPixel; // how far vertically/horizontally the camera moves when the average position of the user's two fingers changes one pixel vertically/horizontally on the screen

    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/pieces/*'/>
    public List<GameObject> pieces; // used in other classes
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/piecesRemovedWhileResettable/*'/>
    public List<GameObject> piecesRemovedWhileResettable; // used in other classes
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/piecesScrollView/*'/>
    public GameObject piecesScrollView; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/pieceControlsPanel/*'/>
    public GameObject pieceControlsPanel; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/pieceRotateLeftButton/*'/>
    public GameObject pieceRotateLeftButton; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/pieceRotateRightButton/*'/>
    public GameObject pieceRotateRightButton; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/pieceRemoveButton/*'/>
    public GameObject pieceRemoveButton; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/cameraRotateLeftButton/*'/>
    public GameObject cameraRotateLeftButton; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/cameraRotateRightButton/*'/>
    public GameObject cameraRotateRightButton; // connected in editor

    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/activePiece/*'/>
    public GameObject activePiece; // used in other classes
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/startStopObject/*'/>
    public GameObject startStopObject; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/resetObject/*'/>
    public GameObject resetObject; // connected in editor
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/clearAllObject/*'/>
    public GameObject clearAllObject; // connected in editor
    private Button startStopButton;
    private Button resetButton;
    private Button clearAllButton;
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/startStopButtonScript/*'/>
    public StartStopButtonBehaviour startStopButtonScript; // used in ResetButtonBehaviour and PieceSourceBehaviour, at least
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/resetButtonScript/*'/>
    public ResetButtonBehaviour resetButtonScript; // used in StartStopButtonBehaviour and PieceSourceBehaviour, at least
    private EventSystem eventSystem;
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/canvas/*'/>
    public GameObject canvas; // connected in editor
    private GraphicRaycaster graphicRaycaster;
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/clearDialogShowing/*'/>
    public bool clearDialogShowing;
    
    // Start is called before the first frame update
    void Start()
    {
        // some basic initializations
        mainCamera = Camera.main;
        currTouchAction = TouchAction.None;
        
        // rotation-related instance variable initializations
        camAxisOfHorizRotation = Vector3.up;
        camAxisOfVertRotation = Vector3.right;
        rotationDegreesPerPixel = 0.1F;
        
        // zoom-related instance variable initializations
        zoomDistPerPixel = 0.01F;

        // panning-related instance variable initializations
        panDistPerPixel = 0.01F;
        
        // initialize lists of pieces the user has added 
        pieces = new List<GameObject>(); // currently active and visible
        piecesRemovedWhileResettable = new List<GameObject>(); // currently inactive and invisible (obsolete if we don't allow piece removal while machine is resettable)
        
        // connect these instance variables to Button components and their scripts
        startStopButton = startStopObject.GetComponent<Button>();
        resetButton = resetObject.GetComponent<Button>();
        clearAllButton = clearAllObject.GetComponent<Button>();
        startStopButtonScript = startStopObject.GetComponent<StartStopButtonBehaviour>();
        resetButtonScript = resetObject.GetComponent<ResetButtonBehaviour>();
        
        // initialize instance variables used for graphic raycasting (i.e. knowing which 2D canvas item was touched)
        eventSystem = GetComponent<EventSystem>();
        graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
        
        // more basic initializing
        clearDialogShowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        // don't want any raycasting detection while a clear all confirmation is showing
        if(clearDialogShowing){
            return; 
        }

        // check how many fingers are touching the screen currently
        if(Input.touchCount == 1){ 
            
            // just one finger is touching the screen
            
            // access that touch
            Touch touch = Input.GetTouch(0);
            
            if(touch.phase == TouchPhase.Began){ 
                // delegate to relevant helper method
                handleFirstTouchBegin(touch);
            }else if(touch.phase == TouchPhase.Moved){
                // delegate to relevant helper method
                handleOneTouchMove(touch);
            }else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled){
                // indicate that all touches have ended and the user is no longer taking any action
                currTouchAction = TouchAction.None;
            }
        }else if(Input.touchCount == 2){
            
            // two fingers are touching the screen
            
            // access both touches
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);
            
            if(secondTouch.phase == TouchPhase.Began){
                // delegate to relevant helper method
                handleSecondTouchBegin(firstTouch, secondTouch);
            }else if(firstTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Moved){
                // delegate to relevant helpder method
                handleTwoTouchMove(firstTouch, secondTouch);
            }else if(firstTouch.phase == TouchPhase.Ended || firstTouch.phase == TouchPhase.Canceled){
                // delegate to relevant helper method, indicating that now only the second touch is occurring
                handleFirstTouchBegin(secondTouch);
            }else if(secondTouch.phase == TouchPhase.Ended || secondTouch.phase == TouchPhase.Canceled){
                // delegate to relevant helper method, indicating that now only the first touch is occurring
                handleFirstTouchBegin(firstTouch);
            }
        }
    }

    private void handleFirstTouchBegin(Touch touch){
        // don't want to detect 3D piece touches while the machine is resettable or while placement correction is occurrring
        bool isPlacementCorrecting = activePiece != null && activePiece.GetComponent<PiecePrefabBehaviour>().isPlacementCorrecting();
        if(!resetButtonScript.getResettable() && !isPlacementCorrecting){ 
            // check if the user touched a significant 3D object
            GameObject hitObject = getSignificant3DObjectHit(touch);
            if(hitObject != null){ 
                // raycast hit a piece
                // indicate that the user is moving a piece
                currTouchAction = TouchAction.MovingPiece;
                // activate the piece
                hitObject.GetComponent<PiecePrefabBehaviour>().OnPieceTouchBegin();
                // return so we don't also end up rotating the camera
                return;
            }
        }

        // check if user touched any Canvas objects
        if(hitCanvasElement(touch)){
            return; // user touched something on the canvas, so don't bother with camera rotation
        }

        // user didn't touch anything (e.g. buttons, scroll bar) on the canvas. 
        // Since we already know user didn't touch a significant 3D piece either, we'll assume they were either starting to rotate the camera or touching the screen to clear their piece selection. 
        // Either way, clear their piece selection and indicate that the user is rotating the camera
        ClearActivePiece();
        currTouchAction = TouchAction.RotatingCamera;
        // store the spot on the screen that the user is currently touching, for the sake of the next frame
        prevFrameFirstTouchPosition = touch.position;
    }

    private void handleOneTouchMove(Touch touch){
        
        // only proceed if we've already determined that the user is rotating the camera
        if(currTouchAction != TouchAction.RotatingCamera){
            return;
        }

        // rotate the camera horizontally 
        mainCamera.gameObject.transform.RotateAround(camRotationCenter.transform.position, camAxisOfHorizRotation, (touch.position - prevFrameFirstTouchPosition).x * rotationDegreesPerPixel);
        // rotate the axis of vertical rotation (a horizontal vector) so that it's still perpendicular to the line of sight
        camAxisOfVertRotation = mainCamera.transform.right;
        // rotate the camera vertically
        mainCamera.gameObject.transform.RotateAround(camRotationCenter.transform.position, camAxisOfVertRotation, (prevFrameFirstTouchPosition - touch.position).y * rotationDegreesPerPixel);
        // we do NOT rotate the axis of horizontal rotation (a vertical vector) to keep it perpendicular to the line of sight; that would end up feeling unnatural
        
        // make sure the camera hasn't been rotated too far vertically
        if(mainCamera.transform.up.y < 0){ 
            // the "top" of the camera is starting to point at all downwards (i.e. at an acute angle to straight down), which we don't want to allow, so undo the vertical rotation
            mainCamera.gameObject.transform.RotateAround(camRotationCenter.transform.position, camAxisOfVertRotation, (touch.position - prevFrameFirstTouchPosition).y * rotationDegreesPerPixel);
        }

        // store the spot on the screen that the user is currently touching, for the sake of the next frame
        prevFrameFirstTouchPosition = touch.position;
    }

    private void handleSecondTouchBegin(Touch firstTouch, Touch secondTouch){
        
        // only proceed if the user is not actively moving a piece
        if(currTouchAction == TouchAction.MovingPiece){
            return;
        }

        // indicate that the user is now zooming/panning the camera
        currTouchAction = TouchAction.ZoomingAndPanningCamera;
        
        // store the spots on the screen that the user is currently touching, for the sake of the next frame
        prevFrameFirstTouchPosition = firstTouch.position;
        prevFrameSecondTouchPosition = secondTouch.position;
    }
    
    private void handleTwoTouchMove(Touch firstTouch, Touch secondTouch){
        
        // only proceed if we've already determined that the user is zooming/panning the camera
        if(currTouchAction != TouchAction.ZoomingAndPanningCamera){
            return;
        }

        // handle zooming, based on the one-frame change in how far apart the user's fingers are on the screen
        float prevTouchDistance = Vector2.Distance(prevFrameFirstTouchPosition, prevFrameSecondTouchPosition);
        float currTouchDistance = Vector2.Distance(firstTouch.position, secondTouch.position);
        mainCamera.gameObject.transform.Translate(mainCamera.transform.forward * (currTouchDistance - prevTouchDistance) * zoomDistPerPixel, Space.World);
        
        // handle panning, based on the one-frame change in the average position of the user's two fingers on the screen
        Vector2 prevAvgTouchPosition = (prevFrameFirstTouchPosition + prevFrameSecondTouchPosition) / 2;
        Vector2 newAvgTouchPosition = (firstTouch.position + secondTouch.position) / 2;
        Vector3 horizComponent = mainCamera.transform.right * (prevAvgTouchPosition - newAvgTouchPosition).x;
        Vector3 vertComponent = mainCamera.transform.up * (prevAvgTouchPosition - newAvgTouchPosition).y;
        Vector3 totalScaledTranslation = (horizComponent + vertComponent) * panDistPerPixel;
        mainCamera.gameObject.transform.Translate(totalScaledTranslation, Space.World);
        // also move the camera's center of rotation the same amount in the same direction
        camRotationCenter.transform.Translate(totalScaledTranslation, Space.World);

        // store the spots on the screen that the user is currently touching, for the sake of the next frame
        prevFrameFirstTouchPosition = firstTouch.position;
        prevFrameSecondTouchPosition = secondTouch.position;
    }

    // casts a ray in the direction of touch and returns the machine piece that is hit, if one is hit
    private GameObject getSignificant3DObjectHit(Touch touch){
        
        // identify a very faraway spot in the direction projected from the user's finger
        Vector3 touchFarWorldCoords = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, mainCamera.farClipPlane));
        
        // set up some more variables before raycasting
        RaycastHit hit3D;
        bool hitSignificant3DObject = false; // "significant" means that this bool will be true for hitting machine pieces but false for hitting a workspace boundary, for example. Before raycasting, assume false
        GameObject hitObject = null;
        
        // Time for raycasting! First, do we hit any 3D objects at all?
        if(Physics.Raycast(mainCamera.gameObject.transform.position, touchFarWorldCoords - mainCamera.gameObject.transform.position, out hit3D)){
            
            // here's the object we hit
            hitObject = hit3D.collider.gameObject;
            
            // assume, for now, that it's a "significant" 3D object
            hitSignificant3DObject = true;
            
            // loop up the hierarchy looking for an ancestor object with a PiecePrefabBehaviour (need to loop like this because of compound colliders)
            while(hitObject.GetComponent<PiecePrefabBehaviour>() == null){
                if(hitObject.transform.parent){
                    // The immediate parent is not null, so next we'll check if the parent object has a PiecePrefabBehaviour
                    hitObject = hitObject.transform.parent.gameObject;
                }else{
                    // We've reached the top of the hierarchy without finding a GameObject with a PiecePrefabBehaviour component. 
                    // This could happen if we had hit the workspace, which has a collider but no ancestors with a PiecePrefabBehaviour.
                    // So, we didn't hit a "significant" 3D object after all
                    hitSignificant3DObject = false;
                    break;
                }
            }
        }

        // By now, we know whether or not we hit a "significant" 3D object (or, indeed, any 3D object at all). Return the corresponding GameObject result
        if(hitSignificant3DObject){
            return hitObject;
        }
        return null;
    }

    // cast a ray towards the canvas in the direction of touch and return a boolean representing whether or not a GameObject on the canvas was hit
    private bool hitCanvasElement(Touch touch){
        
        // set up some variables before raycasting
        PointerEventData touchEventData = new PointerEventData(eventSystem);
        touchEventData.position = touch.position;
        List<RaycastResult> graphicsResults = new List<RaycastResult>(); // this list will be filled with the results from the graphics raycasting
        
        // perform the raycasting
        graphicRaycaster.Raycast(touchEventData, graphicsResults);
        
        // return bool indicating whether or not we hit a graphics object
        return graphicsResults.Count > 0;
    }

    // this method removes the active piece's halo (if there is an active piece), changes its colliders to triggers, sets the activePiece variable to null, and changes the state of canvas objects
    // this method does NOT remove a piece, but it does check if there are any pieces remaining (and deactivates the piece controls panel, etc. if there aren't)
    // it is fine to call this method in this or another class
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/ClearActivePiece/*'/>
    public void ClearActivePiece(){ 
        
        // stuff regarding deactivating the active piece, if there is one currently
        if(activePiece != null){
            
            // disable the halo
            Behaviour halo = activePiece.GetComponent<PiecePrefabBehaviour>().getHalo() as Behaviour;
            halo.enabled = false;
            
            // change the piece's colliders to triggers
            activePiece.GetComponent<PiecePrefabBehaviour>().setTriggers(true);
            
            // update the activePiece instance variable
            activePiece = null;
        }

        // disable the buttons that control the active piece
        if(pieceControlsPanel.activeInHierarchy){
            SetAllPieceControlsButtonsInteractable(false);
        }

        // disable some extra things if there are no pieces left in the machine (i.e. the last piece probably was just removed)
        if(pieces.Count == 0){
            
            // disable the start/stop button
            startStopButton.interactable = false;
            
            // more changes for if the contraption is not resettable (i.e. if it had been un-run or reset, not merely paused)
            // these will always run as long as we prevent piece removal while the contraption is resettable
            if(!resetButtonScript.getResettable()){
                piecesScrollView.SetActive(true);
                startStopButtonScript.setButtonState("start");
                clearAllButton.interactable = false;
            }
        }
    }

    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/SetAllPieceControlsButtonsInteractable/*'/>
    public void SetAllPieceControlsButtonsInteractable(bool active){
        
        // make sure pieceControlsPanel is active so we don't get errors below
        if(!pieceControlsPanel.activeInHierarchy){
            return;
        }

        // set the piece controls buttons to be interactable or not
        pieceRotateLeftButton.GetComponent<Button>().interactable = active;
        pieceRotateRightButton.GetComponent<Button>().interactable = active;
        pieceRemoveButton.GetComponent<Button>().interactable = active;
    }

    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/SetAllCameraButtonsInteractable/*'/>
    public void SetAllCameraButtonsInteractable(bool active){
        // set the camera buttons to be interactable or not
        cameraRotateLeftButton.GetComponent<Button>().interactable = active;
        cameraRotateRightButton.GetComponent<Button>().interactable = active;
    }

    // Set all buttons as interactable or not.
    // Note that the reset button will only have interactable set to true here if the machine is currently resettable, 
    // the start/stop and clear buttons will only have interactable set to true if the machine currently has pieces,
    // and the piece controls buttons will only be set to interactable if there is an active piece (which there might not be, if the user somehow cleared the active piece before placement correction finished)
    /// <include file='docs.xml' path='docs/members[@name="raycasting"]/SetAllButtonsInteractable/*'/>
    public void SetAllButtonsInteractable(bool active){
        SetAllPieceControlsButtonsInteractable(active && activePiece != null); // there needs to be a current active piece for these buttons to be interactable
        SetAllCameraButtonsInteractable(active);
        startStopButton.interactable = active && pieces.Count > 0; // machine needs to have at least one piece for this button to be interactable
        resetButton.interactable = active && resetButtonScript.getResettable(); // machine needs to be resettable for this button to be interactable
        clearAllButton.interactable = active && pieces.Count > 0; // machine needs to have at least one piece for this button to be interactable
    }
}
