using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauchPanel : MonoBehaviour
{
    [SerializeField] public GameObject Login;
    [SerializeField] public GameObject SignUp;

    public void activeLoginPanel()
    {
        Login.gameObject.SetActive(true);
        SignUp.gameObject.SetActive(false);
    }
    public void activeSignUpPanel()
    {
        Login.gameObject.SetActive(false);
        SignUp.gameObject.SetActive(true);
    }
    public void closePanel()
    {
        Login.gameObject.SetActive(false);
        SignUp.gameObject.SetActive(false);
    }
}
