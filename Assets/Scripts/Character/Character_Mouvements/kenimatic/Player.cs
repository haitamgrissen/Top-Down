using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace player
{
    public class Player : MonoBehaviour
    {
        public Transform cameraFollowPoint;
        //public CharacterCamera orbitCamera;
        public Orbit_Camera orbit_Camera;
        public CustomCharacterController character;

        public bool UseMouseInput;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            //orbitCamera.SetFollowTransform(cameraFollowPoint);
            // orbitCamera.IgnoredColliders = character.GetComponentsInChildren<Collider>().ToList();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                Cursor.lockState = CursorLockMode.Locked;

            HandleCameraInput();
            HandlePlayerInput();
        }



        void HandleCameraInput()
        {
            //get the mouse inputs
            float lookaxisUp = Input.GetAxisRaw("Camera Vertical");
            lookaxisUp += UseMouseInput ? Input.GetAxisRaw("Mouse Y") : 0;                 //vertical
            float lookaxisRight = Input.GetAxisRaw("Camera Horizontal");
            lookaxisRight += UseMouseInput ? Input.GetAxisRaw("Mouse X") : 0;   //horizontal
            float zoomInput = UseMouseInput ? Input.GetAxisRaw("Mouse ScrollWheel") : 0;        //scrolling
            Vector3 _lookVector = new Vector3(lookaxisRight, lookaxisUp, 0f);

            if (Cursor.lockState != CursorLockMode.Locked)
                _lookVector = Vector3.zero;
            /*        if (Input.GetMouseButtonDown(1))
                        orbitCamera.TargetDistance = orbitCamera.TargetDistance == 0f ? orbitCamera.DefaultDistance : 0f;*/



            //apply inputs to the camera
            //orbitCamera.UpdateWithInput(Time.deltaTime, zoomInput, _lookVector);
            orbit_Camera.inputVal.x = lookaxisUp;
            orbit_Camera.inputVal.y = lookaxisRight;
            orbit_Camera.projectionSize += zoomInput;
        }

        void HandlePlayerInput()
        {
            PlayerCharacterInputstruct playercharacterinputs = new PlayerCharacterInputstruct();

            playercharacterinputs.MoveAxisForward = Input.GetAxisRaw("Vertical");
            playercharacterinputs.MoveAxisRight = Input.GetAxisRaw("Horizontal");
            playercharacterinputs.CameraRotation = orbit_Camera.transform.rotation; //orbitCamera.transform.rotation;
            playercharacterinputs.JumpDown = Input.GetButtonDown("Jump");
            playercharacterinputs.JumpUp = Input.GetButtonUp("Jump");
            playercharacterinputs.DashingDown = Input.GetKeyDown(KeyCode.Q);

            character.SetInputs(ref playercharacterinputs);
        }
    }
}