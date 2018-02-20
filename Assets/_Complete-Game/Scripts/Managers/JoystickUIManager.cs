using UnityEngine;
using System.Collections;

public class JoystickUIManager : MonoBehaviour
{

	public GameObject LeftStickReference;
	public GameObject RightStickReference; 

	public GameObject player;       // Reference to the player.
	private PlayerHealth playerHealth;

	// Use this for initialization
	void Start ()
	{
		#if !MOBILE_INPUT
			HideReferences();
		#endif
	}


	void HideReferences() {
		LeftStickReference.SetActive (false);
		RightStickReference.SetActive (false);
	}

	void ShowReferences() {
		LeftStickReference.SetActive (true);
		RightStickReference.SetActive (true);
	}
}

