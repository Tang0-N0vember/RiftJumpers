using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CamerController : NetworkBehaviour
{
    public override void OnStartAuthority()
    {
        gameObject.SetActive(true);
    }
}
