using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ItrManager : NetworkBehaviour
{
    public int carPrefabItr=0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    [Command]
    public void UpdateItr(int val)
    {
        carPrefabItr = val;
    }
}
