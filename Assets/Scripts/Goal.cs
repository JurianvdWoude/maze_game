using UnityEngine;

public class Goal : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // let the goal rotate along its y-axis at a fixed speed
        // rotating around multiple axes seems to cause problems. I think that this might be
        // because a gimble lock occurs as two axes the object rotates around overlap 
        Vector3 goalRotation = transform.rotation.eulerAngles + new Vector3(0, Time.deltaTime * 10f, 0);
        transform.rotation = Quaternion.Euler(goalRotation);
    }

    // when the player touches the goal, trigger this event 
    private void OnTriggerEnter(Collider other)
    {
        GameManager.OnFinishMazeGame();
    }
}
