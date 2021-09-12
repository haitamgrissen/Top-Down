using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Character
{

    [RequireComponent(typeof(Rigidbody))]
    public class Character_Mouvements : MonoBehaviour
    {
		[SerializeField, Range(0f, 100f)]
		float maxSpeed = 10f;

		[SerializeField, Range(0f, 100f)]
		float maxAcceleration = 10f, maxAirAcceleration = 1f;

		[SerializeField, Range(0f, 10f)]
		float maxJumpHeight = 2f;
		[SerializeField, Range(0f, 10f)]
		float minJumpHeight = 1f;

		[SerializeField, Range(0f, 5f)]
		float TimeToJumpApex = 2f;

		[SerializeField, Range(0, 5)]
		int maxAirJumps = 0;

		[SerializeField, Range(0, 90)]
		float maxGroundAngle = 25f;

		Rigidbody body;

		Vector3 velocity, desiredVelocity;

		Vector3 contactNormal;

		float gravity;
		float	maxJumpVelocity;
		float	minJumpVelocity;

		bool desiredJump;
		bool releaseJump;

		int groundContactCount;
		Camera maincamera;
		bool OnGround => groundContactCount > 0;

		int jumpPhase;

		float minGroundDotProduct;

		void OnValidate()
		{
			minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
		}

		void calculategravity() 
		{
			gravity = (maxJumpHeight * 2) / (Mathf.Pow(TimeToJumpApex, 2));
			maxJumpVelocity = gravity * TimeToJumpApex;
			minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
			Debug.Log("\ngravity = " + gravity + "maxjumpvelocity = " + maxJumpVelocity + "minjumpvelocity = " + minJumpVelocity);
		}

		void Awake()
		{
			body = GetComponent<Rigidbody>();
			maincamera = Camera.main;
			OnValidate();
			calculategravity();
		}

		void Update()
		{
			Vector2 playerInput;
			playerInput.x = Input.GetAxis("Horizontal");
			playerInput.y = Input.GetAxis("Vertical");
			playerInput = Vector2.ClampMagnitude(playerInput, 1f);

			// camera alignement
			Vector3 targetDirection = new Vector3(playerInput.x, 0f, playerInput.y);
			targetDirection = maincamera.transform.TransformDirection(targetDirection);
			targetDirection.y = 0.0f;


			//desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
			desiredVelocity = targetDirection * maxSpeed;


			desiredJump |= Input.GetButtonDown("Jump");
			releaseJump |= Input.GetButtonUp("Jump");

		}

		void FixedUpdate()
		{
			UpdateState();

			Jump();
			
			AdjustVelocity();

			body.velocity = velocity;
			ClearState();
		}

		void ClearState()
		{
			groundContactCount = 0;
			contactNormal = Vector3.zero;
		}

		void UpdateState()
		{
			velocity = body.velocity;
			if (OnGround)
			{
				jumpPhase = 0;
				if (groundContactCount > 1)
				{
					contactNormal.Normalize();
				}
			}
			else
			{
				contactNormal = Vector3.up;
			}
		}

		void AdjustVelocity()
		{
			Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
			Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

			float currentX = Vector3.Dot(velocity, xAxis);
			float currentZ = Vector3.Dot(velocity, zAxis);

			float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
			float maxSpeedChange = acceleration * Time.deltaTime;

			float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
			float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

			velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
			
			velocity.y += -gravity * Time.deltaTime;
		}

		void Jump()
		{
			if (desiredJump)
			{

				if (OnGround || jumpPhase < maxAirJumps)
				{
					jumpPhase += 1;
					
					float jumpSpeed = maxJumpVelocity;//Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
					float alignedSpeed = Vector3.Dot(velocity, contactNormal);
					if (alignedSpeed > 0f)
					{
						jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
					}
					velocity.y = 0;
					velocity += contactNormal * jumpSpeed;
					//velocity.y = maxJumpVelocity;
				}
			}
            if (releaseJump)
            {
                if (velocity.y > minJumpVelocity)
                    velocity.y = minJumpVelocity;
            }
            desiredJump = false;
				releaseJump = false;
                

                
		}

		void OnCollisionEnter(Collision collision)
		{
			EvaluateCollision(collision);
		}

		void OnCollisionStay(Collision collision)
		{
			EvaluateCollision(collision);
		}

		void EvaluateCollision(Collision collision)
		{
			for (int i = 0; i < collision.contactCount; i++)
			{
				
				Vector3 normal = collision.GetContact(i).normal;
				if (normal.y >= minGroundDotProduct)
				{
					velocity.y = 0;
					groundContactCount += 1;
					contactNormal += normal;
				}
			}
		}

		Vector3 ProjectOnContactPlane(Vector3 vector)
		{
			return vector - contactNormal * Vector3.Dot(vector, contactNormal);
		}
	}
}
