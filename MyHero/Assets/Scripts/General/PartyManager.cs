using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    #region Singleton

    public static PartyManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject hero;
    public GameObject player;
}
