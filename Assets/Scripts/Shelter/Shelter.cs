using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public class shelter : NetworkBehaviour
{
    [Networked] public float Durability { get; set; } = 100f;
    //public Slider DurabilitySlider;


    private void Spawned()
    {
        Durability = 100f; // ��l�Ƴ��S���@�[��
                           //UpdateDurabilityUI();
    }

    public void FixedUpdateNetWork()
    {
        //UpdateDurabilityUI();
    }

    /*private void UpdateDurabilityUI()
    {
        if(DurabilitySlider != null)
        {
            durabilitySilder.value = Durability / 100f;
        }
    }*/
}
