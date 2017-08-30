using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

    [SerializeField]
    private Sprite offCratePointStateSprite;
    [SerializeField]
    private Sprite onCratePointStateSprite;
    private AudioSource myAudio;
    private bool crateState;//false for off, true for On;

    private SpriteRenderer mySpriteRenderer;
    private string wallsTag = "Wall";
    private string cratesTag = "Crate";
    private float tileSize = 1.28f;

	// Use this for initialization
	void Start () {

        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// To swich the Crate state when reach a Crate Point
    /// </summary>
    /// <param name="newState">The new State false for Off, true for On</param>
    public void SwitchStateTo(bool newState)
    {
        if (crateState != newState)
        {
            crateState = newState;
            if (crateState)
            {
                mySpriteRenderer.sprite = onCratePointStateSprite;
            }
            else
            {
                mySpriteRenderer.sprite = offCratePointStateSprite;
            }
        }
    }

    /// <summary>
    /// Check if the Crate can move on
    /// </summary>
    /// <param name="direction">The Vector3 direction value (up, right, down, left)</param>
    /// <returns>true if yes, false if not</returns>
    public bool CanMove(Vector3 direction)
    {
        int myLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.28f, 1);
        if (hit)
        {
            Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, 0), hit.point, Color.red);
            if (hit.transform.tag == wallsTag || hit.transform.tag == cratesTag)//If we hit a wall or another crate, it can'n be moved
            {
                gameObject.layer = myLayer;
                return false;
            }
        }
        gameObject.layer = myLayer;
        return true;
    }

    /// <summary>
    /// Push the crate on the specified direction
    /// </summary>
    /// <param name="pushDirection">The direction of the movement</param>
    /// <param name="pushSpeed">The speed of the movement</param>
    public void Push(int pushDirection,float pushSpeed)
    {
        StartCoroutine(moveCrate(pushDirection, pushSpeed));
    }


    /// <summary>
    /// Move the crate in a "smooth" way
    /// </summary>
    /// <param name="direction">The movement direction</param>
    /// <param name="speed">The movement speed</param>
    /// <returns></returns>
    private IEnumerator moveCrate(int direction, float speed)
    {
        Vector3 startPosition = transform.position;
        float t = 0;


        Vector2 inputValues = getDirectionInputValues(direction);

        Vector3 endPosition = new Vector3(startPosition.x + inputValues.x * tileSize,
                                  startPosition.y + inputValues.y * tileSize,
                                  startPosition.z);

        float factor = 1f;

        myAudio.PlayOneShot(myAudio.clip);

        //Here we control that the position allways match to a multiple of tileSize
        //And depending on the speed, we make the smooth movement;
        while (t < 1f)
        {
            t += Time.deltaTime * (speed / tileSize) * factor;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        yield return 0;
    }

    /// <summary>
    /// Get the input values depending on the direction
    /// </summary>
    /// <param name="direction">The moving direction</param>
    /// <returns>The input/Axis values of that direcion</returns>
    private Vector2 getDirectionInputValues(int direction)
    {
        switch (direction)
        {
            case 1:
                return new Vector2(0, 1);
            case 2:
                return new Vector2(1, 0);
            case 3:
                return new Vector2(0, -1);
            case 4:
                return new Vector2(-1, 0);
        }

        return Vector2.zero;
    }
}
