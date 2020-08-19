using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <include file='docs.xml' path='docs/members[@name="snapToTarget"]/SnapToTargetBehaviour/*'/>
public class SnapToTargetBehaviour : MonoBehaviour
{
    
    /// <include file='docs.xml' path='docs/members[@name="snapToTarget"]/occupied/*'/>
    public bool occupied; // other classes will toggle this
    
    // Start is called before the first frame update
    void Start()
    {
        occupied = false;
    }

}
