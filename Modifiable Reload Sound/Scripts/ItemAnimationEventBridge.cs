using System;
using Audio;
using UnityEngine;

class ItemAnimationEventBridge : MonoBehaviour
{
    private GameObject _item;

    private GameObject Item
    {
        get
        {
            return this._item;
        }
    }

    public void PlayOneShot(string clipName, bool sound_in_head = false)
    {
        if (!sound_in_head)
        {
            Manager.BroadcastPlay(clipName);
            return;
        }
        Manager.PlayInsidePlayerHead(clipName, -1, 0f, false, false);
    }

    public void PlayOneShot(string clipName, Entity entity, bool sound_in_head = false)
    {
        if (!sound_in_head)
        {
            Manager.BroadcastPlay(entity, clipName);
            return;
        }
        Manager.PlayInsidePlayerHead(clipName, entity.entityId);
    }

    public void PlayOneShot(string clipName, Vector3 position)
    {
        Manager.BroadcastPlay(position, clipName);
    }

    public void StopOneShot(string clipName)
    {
        Manager.BroadcastStop(-1, clipName);
    }

    public void StopOneShot(string clipName, Vector3 position)
    {
        Manager.BroadcastStop(position, clipName);
    }

    public void StopOneShot(string clipName, Entity entity)
    {
        Manager.BroadcastStop(entity.entityId, clipName);
    }
}
