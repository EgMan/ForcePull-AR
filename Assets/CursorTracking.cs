using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CursorTracking : MonoBehaviour
{
    public GameObject debug;
    public Material unselected, attracting;
    public bool forcePull = false;
    private Vector3 initialPos;
    private MeshRenderer mesh;
    private void Start()
    {
        initialPos = transform.localPosition;
        mesh = gameObject.GetComponent<MeshRenderer>();
    }
    void Update()
    {
        HandInfo hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        GestureInfo gesture = hand.gesture_info;
        TrackingInfo tracking = hand.tracking_info;
        forcePull = false;
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == 0)
            {
                forcePull = true;
            }
        }

        if (gesture.mano_gesture_continuous != ManoGestureContinuous.NO_GESTURE && gesture.mano_gesture_continuous != 0)
        {
            transform.position = ManoUtils.Instance.CalculateNewPosition(tracking.palm_center, tracking.depth_estimation);
            if (gesture.mano_gesture_continuous == ManoGestureContinuous.CLOSED_HAND_GESTURE)
            {
                forcePull = true;
            }
        }
        else
        {
            transform.localPosition = initialPos;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            forcePull = true;
        }

        if (forcePull)
        {
            mesh.material = attracting;
        }
        else
        {
            mesh.material = unselected;
        }

        var a = debug.GetComponent<Text>();
        a.text = "palm center:"+transform.position.ToString() + "  gesture:"+gesture.mano_gesture_continuous;
    }
}
