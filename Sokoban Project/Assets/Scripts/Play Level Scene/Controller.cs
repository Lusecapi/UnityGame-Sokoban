using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Controller : MonoBehaviour {

    public int speed = 6;

    Animator animatorController;
    Vector2 inputValues;
    int movingDirection;
    float tileSize = 1.28f;

    private string wallsTag = "Wall";
    private string cratesTag = "Crate";

    bool isMoving = false;

	// Use this for initialization
	void Start () {

        animatorController = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        inputValues = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),CrossPlatformInputManager.GetAxis("Vertical"));
        movingDirection = getDirection(inputValues);
        animatorController.SetInteger("MovementDirection", movingDirection);
        if (movingDirection != 0 && !isMoving && CanMove())
        {
            StartCoroutine(move());
        }

    }

    /// <summary>
    /// We set the direction Value depending on the axis iput values
    /// 1 is Up, 2 is Right, 3 is Down and 4 is Left.
    /// </summary>
    /// <param name="inputValue">the Input axis values</param>
    /// <returns></returns>
    private int getDirection(Vector2 inputValue)
    {
        //The only way we can move is when we just move in one direction, won't admit digonal movement
        if(inputValue.x == 0 && inputValue.y != 0)
        {
            if(inputValue.y == 1)
            {
                return 1;
            }
            else
                if(inputValue.y == -1)
            {
                return 3;
            }
        }
        else
            if(inputValue.x != 0 && inputValue.y == 0)
        {
            if(inputValue.x == 1)
            {
                return 2;
            }
            else
                if(inputValue.x == -1)
            {
                return 4;
            }
        }

        return 0;
    }

    /// <summary>
    /// Move the character in a smooth way
    /// </summary>
    /// <returns></returns>
    private IEnumerator move()
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float t = 0;

        Vector3 endPosition = new Vector3(startPosition.x + inputValues.x * tileSize,
                                  startPosition.y + inputValues.y * tileSize,
                                  startPosition.z);

        float factor = 1f;

        //Here we control that the position allways match to a multiple of tileSize
        //And depending on the speed, we make the smooth movement;
        while (t < 1f)
        {
            t += Time.deltaTime * (speed / tileSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        isMoving = false;
        yield return 0;
    }

    /// <summary>
    /// Check if the player is available to move
    /// </summary>
    /// <returns>true if yes, false if not</returns>
    bool CanMove()
    {
        Vector3 direction = Vector3.zero;

        switch (movingDirection)
        {
            case 1:
                direction = Vector3.up;
                break;

            case 2:
                direction = Vector3.right;
                break;

            case 3:
                direction = Vector3.down;
                break;

            case 4:
                direction = Vector3.left;
                break;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.28f, 1);
        if (hit)
        {
            Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 0), hit.point, Color.red);
            if(hit.transform.tag == wallsTag)//If raycast hit something and it's a wall
            {
                return false;
            }
            else
                if(hit.transform.tag == cratesTag)//if hit a crate, we now verify if that crate can move
            {
                Crate theCrate = hit.transform.GetComponent<Crate>();
                if (!theCrate.CanMove(direction))
                {
                    return false;
                }
                else
                {
                    theCrate.Push(movingDirection, speed);
                }
            }
        }
        return true;
    }
}
