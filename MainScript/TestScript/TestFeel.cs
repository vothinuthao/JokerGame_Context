using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class TestFeel : MonoBehaviour
{
    public MMF_Player mmplayer;
    public void OnClickEffect()
    {
        mmplayer.PlayFeedbacks();
    }
}
