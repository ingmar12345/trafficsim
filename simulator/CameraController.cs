using UnityEngine;

/// <summary>
/// Controls the position and rotation of the camera.
/// </summary>
public class CameraController : MonoBehaviour
{
	private const float MovementSpeed = 5.0f;
	private const float RotationSpeed = 20.0f;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKey(KeyCode.D))
		{
            transform.Translate(new Vector3(MovementSpeed * Time.deltaTime, 0, 0), Space.Self);
		}

		if (Input.GetKey(KeyCode.A))
		{
            transform.Translate(new Vector3(-MovementSpeed * Time.deltaTime, 0, 0), Space.Self);
		}

		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
		{
            transform.Translate(new Vector3(0, 0, -MovementSpeed * Time.deltaTime), Space.Self);
		}

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
		{
            transform.Translate(new Vector3(0, 0, MovementSpeed * Time.deltaTime), Space.Self);
        }

		if (Input.GetKey(KeyCode.RightArrow))
		{
			transform.Rotate(new Vector3(0, RotationSpeed * Time.deltaTime, 0));
		}

		if (Input.GetKey(KeyCode.LeftArrow))
		{
			transform.Rotate(new Vector3(0, -RotationSpeed * Time.deltaTime));
		}

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, -RotationSpeed * Time.deltaTime, 0));
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, RotationSpeed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            transform.Translate(new Vector3(0, -MovementSpeed * Time.deltaTime, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.RightBracket))
        {
            transform.Translate(new Vector3(0, MovementSpeed * Time.deltaTime, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            transform.Rotate(new Vector3(RotationSpeed * Time.deltaTime, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            transform.Rotate(new Vector3(-RotationSpeed * Time.deltaTime, 0), Space.Self);
        }
    }
}
