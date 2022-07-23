using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float delayDetach = 2f;
    [SerializeField] private float respawnDelay = 2f;

    private Rigidbody2D currentBallRigidbody2D;
    private SpringJoint2D currentBallSpringJoint2D;


    private Camera mainCamera;
    private bool isDragging;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody2D == null){ return; }
        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            return;
        }
        isDragging = true;
        currentBallRigidbody2D.isKinematic = true;

        Vector2 touchPosition = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }
        touchPosition /= Touch.activeTouches.Count;
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        currentBallRigidbody2D.position = worldPosition;
        Debug.Log("Touch Position : " + touchPosition);
        Debug.Log("World Position : " + worldPosition);
    }

    void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidbody2D = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint2D = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint2D.connectedBody = pivot;
    }

    void LaunchBall()
    {
        Debug.Log("Launching Ball");
        currentBallRigidbody2D.isKinematic = false;
        currentBallRigidbody2D = null;

        Invoke("DetachBall", delayDetach); // invoke with string method
    }

    void DetachBall()
    {
        currentBallSpringJoint2D.enabled = false;
        currentBallSpringJoint2D = null;

        Invoke(nameof(SpawnNewBall), respawnDelay); // invoke name of (alternative)
    }

}
