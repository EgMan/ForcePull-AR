using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public GameObject debug;
    void Update()
    {
        HandInfo hand = ManomotionManager.Instance.Hand_infos[0].hand_info;
        GestureInfo gesture = hand.gesture_info;
        TrackingInfo tracking = hand.tracking_info;

        Debug.Log("palm center "+tracking.palm_center.ToString());
        // transform.position = tracking.palm_center;
        transform.position = ManoUtils.Instance.CalculateNewPosition(tracking.palm_center, tracking.depth_estimation);
        switch (gesture.mano_gesture_continuous)
        {
            case ManoGestureContinuous.CLOSED_HAND_GESTURE:
                break;
            default:
                break;
        }

        var a = debug.GetComponent<Text>();
        a.text = "palm center:"+transform.position.ToString() + "  gesture:"+gesture.mano_gesture_continuous;
    }
}
