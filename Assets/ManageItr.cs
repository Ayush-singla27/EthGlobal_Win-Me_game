using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ManagerItr : NetworkBehaviour
{
    public int carPrefabItr=0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Command]
    public void UpdateItr(int val)
    {
        Debug.Log("gggg" + val);
        carPrefabItr = val;
    }
}
