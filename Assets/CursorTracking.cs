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
    private List<Vector3> handPositionBuffer = new List<Vector3>();
    private int handPositionBufferMaxSize = 4;
    private List<ManoGestureContinuous> handGestureBuffer = new List<ManoGestureContinuous>();
    private int handGestureBufferMaxSize = 4;
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

        // tracking smoothing
        if (handGestureBuffer.Count >= handGestureBufferMaxSize)
        {
            handGestureBuffer.RemoveAt(0);
        }
        handGestureBuffer.Add(gesture.mano_gesture_continuous);
        ManoGestureContinuous smoothedGesture = ManoGestureContinuous.NO_GESTURE;
        foreach (ManoGestureContinuous ges in handGestureBuffer)
        {
            if (ges == ManoGestureContinuous.CLOSED_HAND_GESTURE)
            {
                smoothedGesture = ManoGestureContinuous.CLOSED_HAND_GESTURE;
                break;
            }
            if (ges == ManoGestureContinuous.OPEN_HAND_GESTURE)
            {
                smoothedGesture = ManoGestureContinuous.OPEN_HAND_GESTURE;
            }
        }

        if (smoothedGesture != ManoGestureContinuous.NO_GESTURE)
        {
            // tracking smoothing
            if (handPositionBuffer.Count >= handPositionBufferMaxSize)
            {
                handPositionBuffer.RemoveAt(0);
            }
            handPositionBuffer.Add(ManoUtils.Instance.CalculateNewPosition(tracking.palm_center, tracking.depth_estimation));
            Vector3 smoothedPosition = Vector3.zero;
            foreach (Vector3 pos in handPositionBuffer)
            {
                smoothedPosition += pos;
            }
            smoothedPosition /= handPositionBuffer.Count;

            transform.position = smoothedPosition;
            if (smoothedGesture == ManoGestureContinuous.CLOSED_HAND_GESTURE)
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
