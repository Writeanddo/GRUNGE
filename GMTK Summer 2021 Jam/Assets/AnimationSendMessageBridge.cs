using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSendMessageBridge : MonoBehaviour
{
    public GameObject target;
    public string[] messages;
    public void SendMessageToTarget(int index)
    {
        target.SendMessage(messages[index]);
    }
}
