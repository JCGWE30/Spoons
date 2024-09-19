using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionMenu
{
    MainMenu,
    PlayMenu
}

public class MenuTransition : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject playMenu;

    private TransitionMenu currentDestination = TransitionMenu.PlayMenu;
    private TransitionMenu newDestination;

    public static bool inMotion { get { return instance.isMoving; } }

    private static MenuTransition instance;

    private bool isMoving = false;
    private float startMove;
    private float endMove;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 startRotation;
    private Vector3 endRotation;

    private void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (isMoving)
        {
            float isloatedEnding = endMove - startMove;
            float isolatedTime = Time.time - startMove;

            float percentageComplete = isolatedTime / isloatedEnding;

            if(percentageComplete >= 1)
            {
                Camera.main.transform.position = endPosition;
                isMoving = false;
                GetMarker(currentDestination).parent.gameObject.SetActive(false);
                currentDestination = newDestination;
                return;
            }

            Vector3 value = Vector3.Slerp(startPosition, endPosition, percentageComplete);
            Vector3 rotationValue = Vector3.Slerp(startRotation, endRotation, percentageComplete);

            Camera.main.transform.position = value;
            Camera.main.transform.eulerAngles = rotationValue;
        }
    }

    public static void StartMove(TransitionMenu destination, float moveTime)
    {
        if (inMotion)
            return;

        instance.GetMarker(destination).parent.gameObject.SetActive(true);

        instance.newDestination = destination;

        instance.startPosition = Camera.main.transform.position;
        instance.endPosition = instance.GetMarker(destination).position;

        instance.startRotation = Camera.main.transform.eulerAngles;
        instance.endRotation = instance.GetMarker(destination).eulerAngles;

        instance.isMoving = true;

        instance.startMove = Time.time;
        instance.endMove = Time.time + moveTime;
    }

    private Transform GetMarker(TransitionMenu transition)
    {
        switch (transition)
        {
            case TransitionMenu.MainMenu:
                return mainMenu.transform.Find("CameraMarker");
            case TransitionMenu.PlayMenu:
                return playMenu.transform.Find("CameraMarker");
        }
        return null;
    }
}
