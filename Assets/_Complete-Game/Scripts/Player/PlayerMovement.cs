using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 6f;            // The speed that the player will move at.


        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
#if !MOBILE_INPUT
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#else
		Vector3 moveDir;
		Vector3 fireDir;
#endif

        void Awake ()
        {
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");
#else
			moveDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("MoveX") , 0f , CrossPlatformInputManager.GetAxisRaw("MoveY"));
			fireDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("FireX") , 0f , CrossPlatformInputManager.GetAxisRaw("FireY"));

#endif

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();
        }


        void FixedUpdate ()
        {
            // Store the input axes.
			#if MOBILE_INPUT
				// Virtual axes
	            float h = CrossPlatformInputManager.GetAxisRaw("MoveX");
	            float v = CrossPlatformInputManager.GetAxisRaw("MoveY");
			#else
				// Defined in Input Manager
				float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
				float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
			#endif 

            // Move the player around the scene.
            Move (h, v);

            // Turn the player to face the mouse cursor.
            Turning ();

            // Animate the player.
            Animating (h, v);
        }


        void Move (float h, float v)
        {
            // Set the movement vector based on the axis input.
            movement.Set (h, 0f, v);
            
            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition (transform.position + movement);
        }


        void Turning ()
        {
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation (newRotatation);
            }
#else
			// Update move and fire directions
			moveDir.x = CrossPlatformInputManager.GetAxisRaw("MoveX"); 
			moveDir.y = 0f;
			moveDir.z = CrossPlatformInputManager.GetAxisRaw("MoveY");

			fireDir.x= CrossPlatformInputManager.GetAxisRaw("FireX");
			fireDir.y = 0f; 
			fireDir.z = CrossPlatformInputManager.GetAxisRaw("FireY");

			// Check for input on the fire direction, if none then the move direction sets the turn
			if (fireDir == Vector3.zero) {
	            if ( moveDir != Vector3.zero)
	            {
	                // Create a vector from the player to the direction of movement.
	                Vector3 playerToMouse = (transform.position + moveDir) - transform.position;

	                // Ensure the vector is entirely along the floor plane.
	                playerToMouse.y = 0f;

	                // Create a quaternion (rotation) based on the direction of movement.
	                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

	                // Set the player's rotation to this new rotation.
	                playerRigidbody.MoveRotation(newRotatation);
	            }
			}
			else {
				// Create a vector from the player to the direction of fire.
				Vector3 playerToMouse = (transform.position + fireDir) - transform.position;

				// Ensure the vector is entirely along the floor plane.
				playerToMouse.y = 0f;

				// Create a quaternion (rotation) based on the direction of fire.
				Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

				// Set the player's rotation to this new rotation.
				playerRigidbody.MoveRotation(newRotatation);
			}


#endif
        }


        void Animating (float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool ("IsWalking", walking);
        }
    }
}