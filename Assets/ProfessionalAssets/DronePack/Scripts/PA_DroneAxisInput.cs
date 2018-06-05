using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class PA_DroneAxisInput : MonoBehaviour
    {

        #region Variables
        public enum InputType { Desktop, Gamepad, OpenVR, Custom }
        public InputType inputType = InputType.Desktop;

        public string forwardBackward;
        public string strafeLeftRight;
        public string riseLower;
        public string turn;

        public string toggleMotor;
        public string toggleCameraMode;
        public string toggleCameraGyro;
        public string toggleFollowMode;

        public string cameraRiseLower;
        public string cameraTurn;
        public string cameraFreeLook;

        public string cforwardBackward;
        public string cstrafeLeftRight;
        public string criseLower;
        public string cturn;

        public string cCameraRiseLower;
        public string cCameraTurn;
        public string cCameraFreeLook;

        public string ctoggleMotor;
        public string ctoggleCameraMode;
        public string ctoggleCameraGyro;
        public string ctoggleFollowMode;
        #endregion

        #region Hidden Variables
        public PA_DroneController dcoScript;
        public PA_DroneCamera dcScript;

        bool toggleMotorIsKey = false;
        bool toggleMotorIsAxis = false;

        bool toggleCameraModeIsKey = false;
        bool toggleCameraModeIsAxis = false;

        bool toggleCameraGyroIsKey = false;
        bool toggleCameraGyroIsAxis = false;

        bool toggleFollowModeIsKey = false;
        bool toggleFollowModeIsAxis = false;

        bool cameraFreeLookIsKey = false;
        bool cameraFreeLookIsAxis = false;
        #endregion
        //bool validated = false;


        void Awake()
        {
            #region Cache Components + Custom Input
            dcoScript = GetComponent<PA_DroneController>();
            dcScript = FindObjectOfType<PA_DroneCamera>();
            if (inputType == InputType.Custom)
            {
                forwardBackward = cforwardBackward;
                strafeLeftRight = cstrafeLeftRight;
                riseLower = criseLower;
                turn = cturn;
                toggleMotor = ctoggleMotor;
                toggleCameraMode = ctoggleCameraMode;
                toggleCameraGyro = ctoggleCameraGyro;
                toggleFollowMode = ctoggleFollowMode;
                cameraRiseLower = cCameraRiseLower;
                cameraTurn = cCameraTurn;
                cameraFreeLook = cCameraFreeLook;
            }
            #endregion
        }

        void Start()
        {
            #region Validate Buttons/Keycodes
            ValidateInputs();
            #endregion
        }

        void Update()
        {
            #region Input Axis Listeners
            if (forwardBackward != "")
            {
                dcoScript.DriveInput(Input.GetAxisRaw(forwardBackward));
            }

            if (strafeLeftRight != "")
            {
                dcoScript.StrafeInput(Input.GetAxisRaw(strafeLeftRight));
            }

            if (riseLower != "")
            {
                dcoScript.LiftInput(Input.GetAxisRaw(riseLower));
            }

            if (turn != "")
            {
                dcoScript.TurnInput(Input.GetAxis(turn));
            }

            if(cameraRiseLower != "")
            {
                dcScript.LiftInput(Input.GetAxis(cameraRiseLower));
            }

            if (cameraTurn != "")
            {
                dcScript.TurnInput(Input.GetAxis(cameraTurn));
            }
            #endregion

            #region Button / KeyCode Listeners
            if (toggleMotor != "")
            {
                if (toggleMotorIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), toggleMotor))) { dcoScript.ToggleMotor(); }
                }
                if (toggleMotorIsAxis)
                {
                    if (Input.GetButtonDown(toggleMotor)) { dcoScript.ToggleMotor(); }
                }
            }

            if (toggleCameraMode != "" && dcScript)
            {
                if (toggleCameraModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), toggleCameraMode))) { dcScript.ChangeCameraMode(); }
                }
                if (toggleCameraModeIsAxis)
                {
                    if (Input.GetButtonDown(toggleCameraMode)) { dcScript.ChangeCameraMode(); }
                }
            }

            if (toggleCameraGyro != "" && dcScript)
            {
                if (toggleCameraGyroIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), toggleCameraGyro))) { dcScript.ChangeGyroscope(); }
                }
                if (toggleCameraGyroIsAxis)
                {
                    if (Input.GetButtonDown(toggleCameraGyro)) { dcScript.ChangeGyroscope(); }
                }
            }

            if (toggleFollowMode != "" && dcScript)
            {
                if (toggleFollowModeIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), toggleFollowMode))) { dcScript.ChangeFollowMode(); }
                }
                if (toggleFollowModeIsAxis)
                {
                    if (Input.GetButtonDown(toggleFollowMode)) { dcScript.ChangeFollowMode(); }
                }
            }

            if (cameraFreeLook != "" && dcScript)
            {
                if (cameraFreeLookIsKey)
                {
                    if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), cameraFreeLook))) { dcScript.CanFreeLook(true); }
                    if (Input.GetKeyUp((KeyCode)System.Enum.Parse(typeof(KeyCode), cameraFreeLook))) { dcScript.CanFreeLook(false); }
                }
                if (cameraFreeLookIsAxis)
                {
                    if (Input.GetButtonDown(cameraFreeLook)) { dcScript.CanFreeLook(true); }
                    if (Input.GetButtonUp(cameraFreeLook)) { dcScript.CanFreeLook(false); }
                }
            }
            #endregion
        }

        #region Custom Validation Functions

        void ValidateInputs()
        {
            if (toggleMotor != "")
            {
                if (ValidKey(toggleMotor)) { toggleMotorIsKey = true; }
                if (ValidAxis(toggleMotor)) { toggleMotorIsAxis = true; }
                if (!toggleMotorIsKey && !toggleMotorIsAxis) { Debug.LogWarning("PA_DroneAxisInput : '" + toggleMotor + "' is not a valid Keycode or Input Axis, it will not be used"); }
            }

            if (toggleCameraMode != "")
            {
                if (ValidKey(toggleCameraMode)) { toggleCameraModeIsKey = true; }
                if (ValidAxis(toggleCameraMode)) { toggleCameraModeIsAxis = true; }
                if (!toggleCameraModeIsKey && !toggleCameraModeIsAxis) { Debug.LogWarning("PA_DroneAxisInput : '" + toggleCameraMode + "' is not a valid Keycode or Input Axis, it will not be used"); }
            }

            if (toggleCameraGyro != "")
            {
                if (ValidKey(toggleCameraGyro)) { toggleCameraGyroIsKey = true; }
                if (ValidAxis(toggleCameraGyro)) { toggleCameraGyroIsAxis = true; }
                if (!toggleCameraGyroIsKey && !toggleCameraGyroIsAxis) { Debug.LogWarning("PA_DroneAxisInput : '" + toggleCameraGyro + "' is not a valid Keycode or Input Axis, it will not be used"); }
            }

            if (toggleFollowMode != "")
            {
                if (ValidKey(toggleFollowMode)) { toggleFollowModeIsKey = true; }
                if (ValidAxis(toggleFollowMode)) { toggleFollowModeIsAxis = true; }
                if (!toggleFollowModeIsKey && !toggleFollowModeIsAxis) { Debug.LogWarning("PA_DroneAxisInput : '" + toggleFollowMode + "' is not a valid Keycode or Input Axis, it will not be used"); }
            }

            if (cameraFreeLook != "")
            {
                if (ValidKey(cameraFreeLook)) { cameraFreeLookIsKey = true; }
                if (ValidAxis(cameraFreeLook)) { cameraFreeLookIsAxis = true; }
                if (!cameraFreeLookIsKey && !cameraFreeLookIsAxis) { Debug.LogWarning("PA_DroneAxisInput : '" + cameraFreeLook + "' is not a valid Keycode or Input Axis, it will not be used"); }
            }
        }

        bool ValidKey(string btnName)
        {
            try { Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), btnName)); return true; } catch (System.Exception) { return false; }
        }

        bool ValidAxis(string axsName)
        {
            try { Input.GetAxis(axsName); return true; } catch (System.Exception) { return false; }
        }
        #endregion

    }
}
