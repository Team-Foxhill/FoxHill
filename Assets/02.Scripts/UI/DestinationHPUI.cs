using FoxHill.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestinationHPUI : MonoBehaviour
{
    [SerializeField] private Image _hp;
    public void HPBar(float amount)
    {
        _hp.fillAmount = amount;
    }
}
