using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtons//¦Û­q¸q«ö¶s
{
    FIRE
}

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons buttons;

    public Vector2 movementInput;
    public Vector2 mousePosition;
}