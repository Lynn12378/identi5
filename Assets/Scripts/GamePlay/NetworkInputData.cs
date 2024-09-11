using UnityEngine;
using Fusion;

namespace Identi5.GamePlay
{
    public enum InputButtons
    {
        FIRE,
        SPACE,
        TALK,
        RELOAD,
        PET
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons buttons;
        public Vector2 movementInput;
        public Vector2 mousePosition;
    }
}