﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour , IPointerUpHandler , IPointerDownHandler , IDragHandler {

    public int MovementRange = 100;

    public enum AxisOption
    {                                                    // Options for which axes to use                                                     
        Both,                                                                   // Use both
        OnlyHorizontal,                                                         // Only horizontal
        OnlyVertical                                                            // Only vertical
    }

    public AxisOption axesToUse = AxisOption.Both;   // The options for the axes that the still will use
    public string horizontalAxisName;// The name given to the horizontal axis for the cross platform input
    public string verticalAxisName;    // The name given to the vertical axis for the cross platform input 

    private Vector3 startPos;
    private bool useX;                                                          // Toggle for using the x axis
    private bool useY;                                                          // Toggle for using the Y axis
    private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;               // Reference to the joystick in the cross platform input
    private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;                 // Reference to the joystick in the cross platform input
    

	private Vector3 targetVec;			// Vector in direction where user has dragged a joystick	
	private Vector3 draggedPosition;	// Stores position for where joystick has been dragged to

    void OnEnable () {

		startPos = transform.position;
		targetVec = new Vector3 (0f, 0f, 0f);
		draggedPosition = new Vector3 (0f, 0f, 0f);
        CreateVirtualAxes ();
    }

    private void UpdateVirtualAxes (Vector3 value) {

        var delta = startPos - value;
        delta.y = -delta.y;
        delta /= MovementRange;
        if(useX)
        horizontalVirtualAxis.Update (-delta.x);

        if(useY)
        verticalVirtualAxis.Update (delta.y);

    }

    private void CreateVirtualAxes()
    {
        // set axes to use
        useX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
        useY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

        // create new axes based on axes to use
        if (useX)
            horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
        if (useY)
            verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
    }


    public void OnDrag(PointerEventData data) {
        Vector3 newPos = Vector3.zero;

		// Effectively follows the approach of the individual clamp approach but maintains
		// Direction
		if (useY && useX) {
			
			targetVec.x = data.position.x - startPos.x;
			targetVec.y = data.position.y - startPos.y; 
			targetVec.z = 0f; 
				
			targetVec = targetVec.normalized;		

			newPos.x = MovementRange*targetVec.x;
			newPos.y = MovementRange*targetVec.y;
		}  
		else 
		{
			if (useX) {
				int delta = (int) (data.position.x - startPos.x);
				delta = Mathf.Clamp(delta,  - MovementRange,  MovementRange);
				newPos.x = delta;
			}

			if (useY)
			{
				int delta = (int)(data.position.y - startPos.y);
				delta = Mathf.Clamp(delta, -MovementRange,  MovementRange);
				newPos.y = delta;
			}
		}


		// Recycling dragged position vector, updating with where user has dragged joystick to
		draggedPosition.x = startPos.x + newPos.x;
		draggedPosition.y = startPos.y + newPos.y;
		draggedPosition.z = startPos.z + newPos.z;
		transform.position  = draggedPosition;
		UpdateVirtualAxes (transform.position);



    }


    public  void OnPointerUp(PointerEventData data)
    {

        transform.position = startPos;
        UpdateVirtualAxes (startPos);
    }


    public  void OnPointerDown (PointerEventData data) {
    }

    void OnDisable () {
        // remove the joysticks from the cross platform input
        if (useX)
        {
            horizontalVirtualAxis.Remove();
        }
        if (useY)
        {
            verticalVirtualAxis.Remove();
        }
    }
}
