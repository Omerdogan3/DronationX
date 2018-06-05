using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PA_DronePack
{
    public class PA_DroneController : MonoBehaviour
    {
        #region Input Type
        public enum InputType { Desktop, Mobile, Gamepad, Vive }
        public InputType inputType = InputType.Desktop;
        #endregion

        #region Movement Values
        public float forwardSpeed = 7f;
        public float backwardSpeed = 5f;
        public float leftSpeed = 5f;
        public float rightSpeed = 5f;
        public float riseSpeed = 5f;
        public float lowerSpeed = 5f;

        public float acceleration = 0.5f;
        public float deceleration = 0.2f;
        public float stability = 0.1f;
        public float turnSensitivty = 2f;

        public bool motorOn = true;
        #endregion

        #region Appearance
        //public GameObject droneBody; // Variable Depreciated; Reason : Emission values can be controled directly on the StandardShader Material
        public List<GameObject> propellers;
        public float propSpinSpeed = 50;
        [Range(0, 1f)]
        public float propStopSpeed = 1f;
        public Transform frontTilt, backTilt, rightTilt, leftTilt;
        #endregion

        #region Collision Settings
        public bool fallAfterCollision = true;
        public float fallMinimumForce = 6f;
        public float sparkMinimumForce = 1f;
        public GameObject sparkPrefab;
        #endregion

        #region Sound Effects
        public AudioSource flyingSound;
        public AudioSource sparkSound;
        #endregion

        #region Read Only Values
        public float collisionMagnitude;
        public float liftForce, driveForce, strafeForce, turnForce;
        public float groundDistance = Mathf.Infinity;
        public float uprightAngleDistance;
        public float calPropSpeed = 0;
        public Vector3 startPosition;
        public Quaternion startRotation;
        #endregion

        #region Hidden Variables
        public Rigidbody rigidBody;
        public PA_DroneAxisInput usingAxisInput;
        public float driveInput = 0, strafeInput = 0, liftInput = 0;
        public RaycastHit hit;
        #endregion

        void Awake()
        {
            #region Cache Components & Start Values
            rigidBody = GetComponent<Rigidbody>();
            usingAxisInput = GetComponent<PA_DroneAxisInput>();
            startPosition = transform.position;
            startRotation = transform.rotation;
            #endregion
        }

        void Update()
        {
            #region Float Calculations
            uprightAngleDistance = (1f - transform.up.y) * 0.01f;
            uprightAngleDistance = (uprightAngleDistance < 0.001) ? 0f : uprightAngleDistance;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity)) { groundDistance = hit.distance; }
            #endregion

            #region Propellers
            calPropSpeed = (motorOn) ? propSpinSpeed : calPropSpeed * (1f - (propStopSpeed / 2));
            foreach (GameObject propeller in propellers) { propeller.transform.Rotate(0, 0, calPropSpeed); }
            #endregion

            #region Sounds
            if (flyingSound)
            {
                flyingSound.volume = (calPropSpeed / propSpinSpeed);
                flyingSound.pitch = 1 + (liftForce * 0.02f);
            }
            #endregion
        }

        void FixedUpdate()
        {
            #region Gravity
            rigidBody.useGravity = (motorOn) ? false : true;
            #endregion

            if (motorOn)
            {
                #region Tilt
                if (groundDistance > 0.2f)
                {
                    if (driveInput > 0) { rigidBody.AddForceAtPosition(Vector3.down * Mathf.Abs(driveForce * (rigidBody.mass * 0.1f)), frontTilt.position); }
                    if (driveInput < 0) { rigidBody.AddForceAtPosition(Vector3.down * Mathf.Abs(driveForce * (rigidBody.mass * 0.1f)), backTilt.position); }
                    if (strafeInput > 0) { rigidBody.AddForceAtPosition(Vector3.down * Mathf.Abs(strafeForce * (rigidBody.mass * 0.1f)), rightTilt.position); }
                    if (strafeInput < 0) { rigidBody.AddForceAtPosition(Vector3.down * Mathf.Abs(strafeForce * (rigidBody.mass * 0.1f)), leftTilt.position); }
                }
                #endregion

                #region Movement & Rotation
                Vector3 localVelocity = transform.InverseTransformDirection(rigidBody.velocity);

                localVelocity.z = (driveInput != 0) ? Mathf.Lerp(localVelocity.z, driveInput, acceleration * 0.3f) : Mathf.Lerp(localVelocity.z, driveInput, deceleration * 0.2f);
                driveForce = (Mathf.Abs(localVelocity.z) > 0.01f) ? localVelocity.z : 0f;
                localVelocity.x = (strafeInput != 0) ? Mathf.Lerp(localVelocity.x, strafeInput, acceleration * 0.3f) : Mathf.Lerp(localVelocity.x, strafeInput, deceleration * 0.2f);
                strafeForce = (Mathf.Abs(localVelocity.x) > 0.01f) ? localVelocity.x : 0f;

                rigidBody.velocity = transform.TransformDirection(localVelocity);

                liftForce = (liftInput != 0) ? Mathf.Lerp(liftForce, liftInput, acceleration * 0.2f) : Mathf.Lerp(liftForce, liftInput, deceleration * 0.3f);
                liftForce = (Mathf.Abs(liftForce) > 0.01f) ? liftForce : 0f;

                rigidBody.velocity = new Vector3(rigidBody.velocity.x, liftForce, rigidBody.velocity.z);

                rigidBody.angularVelocity *= 1f - (stability * (1.4f / (rigidBody.mass + transform.lossyScale.x)));

                float calStability = (uprightAngleDistance * ((((rigidBody.mass * 1.2f) + transform.lossyScale.x) * 14.28f) + stability));
                calStability = Mathf.Clamp(calStability, (((rigidBody.mass * 1.2f) + transform.lossyScale.x) * 0.714f), Mathf.Infinity);

                Quaternion uprightRotation = Quaternion.FromToRotation(transform.up, Vector3.up);
                rigidBody.AddTorque(new Vector3(uprightRotation.x, 0, uprightRotation.z) * calStability);
                rigidBody.angularVelocity = new Vector3(rigidBody.angularVelocity.x, turnForce, rigidBody.angularVelocity.z);
                #endregion
            }
        }

        void OnCollisionEnter(Collision newObject)
        {
            #region Sparks & Falling & Sound
            collisionMagnitude = newObject.relativeVelocity.magnitude;
            if (collisionMagnitude > sparkMinimumForce)
            {
                SpawnSparkPrefab(newObject.contacts[0].point);
                if (sparkSound) { sparkSound.pitch = collisionMagnitude * 0.1f; sparkSound.PlayOneShot(sparkSound.clip, collisionMagnitude * 0.05f); }
            }
            if (collisionMagnitude > fallMinimumForce && fallAfterCollision)
            {
                motorOn = false;
            }
            #endregion
        }

        void OnCollisionStay(Collision newObject)
        {
            #region Prevents Glitchy Downward Movement
            if (Physics.Raycast(transform.position, Vector3.down, out hit, GetComponent<Collider>().bounds.extents.y + 0.15f))
            {
                liftForce = Mathf.Clamp(liftForce, 0, Mathf.Infinity);
            }
            #endregion
        }

        #region Custom Functions
        public void ToggleMotor() { motorOn = !motorOn; }
        public void DriveInput(float input) { if (input > 0) { driveInput = input * forwardSpeed; } else if (input < 0) { driveInput = input * backwardSpeed; } else { driveInput = 0; } }
        public void StrafeInput(float input) { if (input > 0) { strafeInput = input * rightSpeed; } else if (input < 0) { strafeInput = input * leftSpeed; } else { strafeInput = 0; } }
        public void LiftInput(float input) { if (input > 0) { liftInput = input * riseSpeed; motorOn = true; } else if (input < 0) { liftInput = input * lowerSpeed; } else { liftInput = 0; } }
        public void TurnInput(float input) { turnForce = input * turnSensitivty; }
        public void ResetDronePosition() { rigidBody.position = startPosition; rigidBody.rotation = startRotation; rigidBody.velocity = Vector3.zero; }
        public void SpawnSparkPrefab(Vector3 position) { GameObject spark = Instantiate(sparkPrefab, position, Quaternion.identity) as GameObject; ParticleSystem.MainModule ps = spark.GetComponent<ParticleSystem>().main; Destroy(spark, ps.duration + ps.startLifetime.constantMax); }

        public void AdjustLift(float value) { riseSpeed = value; lowerSpeed = value; }
        public void AdjustSpeed(float value) { forwardSpeed = value; backwardSpeed = value; }
        public void AdjustStrafe(float value) { rightSpeed = value; leftSpeed = value; }
        public void AdjustTurn(float value) { turnSensitivty = value; }
        public void AdjustAccel(float value) { acceleration = value; }
        public void AdjustDecel(float value) { deceleration = value; }
        public void AdjustStable(float value) { stability = value; }
        public void ToggleFall(bool state) { fallAfterCollision = !fallAfterCollision; }

        public void ChangeFlightAudio(AudioClip newClip) { flyingSound.clip = newClip; flyingSound.enabled = false; flyingSound.enabled = true; }
        public void ChangeImpactAudio(AudioClip newClip) { sparkSound.clip = newClip; sparkSound.enabled = false; sparkSound.enabled = true; }
        #endregion
    }
}