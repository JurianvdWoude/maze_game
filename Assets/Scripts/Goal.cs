using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.finishedGame = true;
        GameManager.playGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goalRotation = transform.rotation.eulerAngles + new Vector3(0, Time.deltaTime * 10f, 0);
        transform.rotation = Quaternion.Euler(goalRotation);
    }
}
