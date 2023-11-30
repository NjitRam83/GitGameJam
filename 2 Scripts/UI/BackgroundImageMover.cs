using UnityEngine;

public class BackgroundImageMover : MonoBehaviour
{
    public float seconds;
    public float timer;
    public Vector3 Point;
    public Vector3 Difference;
    public Vector3 start;
    public float percent;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        Difference = new Vector3(1, 0, 0); 
    }

    // Update is called once per frame
    void Update()
    {

        if (timer <= seconds)
        {
            // basic timer
            timer += Time.deltaTime;
            // percent is a 0-1 float showing the percentage of time that has passed on our timer!
            percent = timer / seconds;
            // multiply the percentage to the difference of our two positions
            // and add to the start
            transform.position = start + Difference * percent * Time.deltaTime;
        }
    }
}
