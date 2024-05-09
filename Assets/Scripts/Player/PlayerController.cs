using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
    [SerializeField] private PlayerMovementHandler movementHandler = null;
    [SerializeField] private PlayerAttackHandler attackHandler = null;

    [SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private Image hpBar = null;

    [Networked] public int Hp { get; set; } //(OnChanged = nameof(OnHpChanged)),Networked, OnChangedRender(nameof(OnColorChanged))
    [Networked] private NetworkButtons buttonsPrevious { get; set; }

    private int maxHp = 100;

    public override void Spawned()//初始化
    {
        if (Object.HasStateAuthority)
        {
            Hp = maxHp;
        }
    }
    private void Respawn()//重生
    {
        playerNetworkRigidbody.transform.position = Vector3.zero;
        Hp = maxHp;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            ApplyInput(data);
        }

        if (Hp <= 0)
        {
            Respawn();
        }
    }

    private void ApplyInput(NetworkInputData data)
    {
        NetworkButtons buttons = data.buttons;
        var pressed = buttons.GetPressed(buttonsPrevious);
        buttonsPrevious = buttons;

        movementHandler.Move(data);

        if (pressed.IsSet(InputButtons.FIRE))
        {
            attackHandler.Shoot(data.mousePosition);
        }
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            Hp -= damage;
        }
    }
    
    /*private static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }*/
}

