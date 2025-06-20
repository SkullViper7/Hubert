using System.Collections;
using UnityEngine;

public class LaserMoving : MonoBehaviour
{
    public Transform targetPoint;
    public float laserSpeed = 2f;
    public float waitTimeAtEnd = 1f;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 destination;
    private bool isWaiting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPoint = transform.position;
        endPoint = targetPoint.position;
        destination = endPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaiting) return;

        transform.position = Vector3.MoveTowards(transform.position, destination, laserSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.001f)
        {
            StartCoroutine(WaitAndSwitchDirection());
        }

        IEnumerator WaitAndSwitchDirection()
        {
            isWaiting = true;
            yield return new WaitForSeconds(waitTimeAtEnd);

            destination = (destination == startPoint) ? endPoint : startPoint;
            isWaiting = false;
        }

    }
}
