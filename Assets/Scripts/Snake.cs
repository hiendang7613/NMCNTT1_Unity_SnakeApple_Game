using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]

public class Snake : MonoBehaviour
{
    // Each snake segment
    private List<Transform> _segments;

    public static int score;

    // snake segment image
    public Transform segmentPrefab;
    public Transform snake_L_Left;
    public Transform snake_L_Right;
    public Transform snake_tail;

    public int initialSize = 0;

    [HideInInspector]
    public Vector2 direction = Vector2.right;

    // Start game
    private void Start()
    {
        _segments = new List<Transform>();
        ResetState();
    }

    // Hook key 
    private void Update()
    {
        // If moving horizontal, then only allow turning up or down
        if (this.direction.x != 0.0f)
        {
            // Set the direction based on the input key being pressed
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                this.transform.Rotate (0, 0, 90*this.direction.x);
                this.direction = Vector2.up;

            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                this.transform.Rotate (0, 0, -90*this.direction.x);
                this.direction = Vector2.down;

            }
        }
        // If moving vertical, then only allow turning left or right
        else if (this.direction.y != 0.0f)
        {
            // Set the direction based on the input key being pressed
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                this.transform.Rotate (0, 0, -90*this.direction.y);
                this.direction = Vector2.right;

            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                this.transform.Rotate (0, 0,90*this.direction.y);
                this.direction = Vector2.left;

            }
        }
    }

    // render snake each move step 
    private void FixedUpdate()
    {
        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].position = _segments[i - 1].position;
        }

        // Increase the snake's position by one in the direction they are
        // moving. Round the position to ensure it stays aligned to the grid.
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + this.direction.x,
            Mathf.Round(this.transform.position.y) + this.direction.y
            );

        Transform segment;
        for (int i = _segments.Count - 2; i > 0; i--) {

            if ((_segments[i+1].position.x-_segments[i-1].position.x)!=0&&(_segments[i+1].position.y-_segments[i-1].position.y)!=0){
                var val = (_segments[i+1].position.x-_segments[i].position.x)*(_segments[i].position.y-_segments[i-1].position.y)+(_segments[i+1].position.y-_segments[i].position.y)*(_segments[i].position.x-_segments[i-1].position.x);
                Debug.Log(val);
                if(val<0){
                    if((_segments[i].position.y-_segments[i+1].position.y)==0){
                        segment = Instantiate(this.snake_L_Right);
                    }else{
                        segment = Instantiate(this.snake_L_Left);
                    }
                }else{
                    if((_segments[i].position.x-_segments[i+1].position.x)==0){
                        segment = Instantiate(this.snake_L_Right);
                    }else{
                        segment = Instantiate(this.snake_L_Left);
                    }
                }
                segment.position = _segments[i].position;
            }else{
                segment = Instantiate(this.segmentPrefab);
                segment.position = _segments[i].position;
            }

            var dx = _segments[i].position.x-_segments[i+1].position.x;
            var dy = _segments[i].position.y-_segments[i+1].position.y;

            var angle = 45*(   Mathf.Pow(1-dx,2)+Mathf.Abs(dy)   )*(dy!=0?dy:1) ;
            segment.transform.Rotate (0, 0,   angle);
            Destroy(_segments[i].gameObject);
            _segments[i] =segment;
        }
        segment = Instantiate(this.snake_tail);

        segment.position = _segments[_segments.Count - 1].position;
        var dx2 = _segments[_segments.Count - 2].position.x-_segments[_segments.Count - 1].position.x;
        var dy2 = _segments[_segments.Count - 2].position.y-_segments[_segments.Count - 1].position.y;

        var angle2 = 45*(   Mathf.Pow(1-dx2,2)+Mathf.Abs(dy2)   )*(dy2!=0?dy2:1) ;
        segment.transform.Rotate (0, 0,   angle2);
        Destroy(_segments[_segments.Count - 1].gameObject);
        _segments[_segments.Count - 1]= segment;

    }

    // snake grow
    public void Grow()
    {
        // Create a new segment
        Transform segment = Instantiate(this.segmentPrefab);

        // Position the segment at the same spot as the last segment
        segment.position = _segments[_segments.Count - 1].position;

        // Add the segment to the end of the list
        _segments.Add(_segments[_segments.Count - 1]);
        _segments[_segments.Count - 2] = segment;
    }

    // Reset game
    public void ResetState()
    {
        // Set the initial direction of the snake, starting at the origin
        // (center of the grid)
        this.transform.Rotate (0, 0,   45*(   Mathf.Pow(1-this.direction.x,2)+Mathf.Abs(this.direction.y)   )*(this.direction.y!=0?-this.direction.y:1) );
        //this.transform.Rotate (0, 0,90 );
        //Debug.Log(45*(  Mathf.Pow(1-this.direction.x,2)*(1-this.direction.x)-this.direction.y   ));

        this.direction = Vector2.right;
        this.transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }

        // Clear the list then add the head (this) as the first segment
        _segments.Clear();
        _segments.Add(this.transform);

        Transform segment = Instantiate(this.snake_tail);
        segment.position = _segments[0].position;
        _segments.Add(segment);

        // Grow the snake to the initial size -1 since the head was already
        // added
        for (int i = 0; i < this.initialSize - 1; i++) {
            Grow();
        }
    }

    // collision event 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            // Food eaten, increase the size of the snake
            Grow();
        }
        else if (other.tag == "Obstacle")
        {
            // Game over, reset the state of the snake
            //Debug.Log(other);


            //ResetState();
            score = _segments.Count;
            _segments.Clear();
            new WaitForSeconds(2);
            SceneManager.LoadScene(4);

        }
    }

}
