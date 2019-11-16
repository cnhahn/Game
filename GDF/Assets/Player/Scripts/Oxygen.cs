using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Oxygen : MonoBehaviour
{
    [SerializeField]
    private float _oxygenLevel;
    [SerializeField]
    private float _maxOxygen = 100;
    [SerializeField]
    private float _oxygenConsumptionRate;
    [SerializeField]
    private float _oxygenRechargeRate;
    [SerializeField]
    private bool _isded;
    [SerializeField]
    private RectTransform _uiPanel;

    private bool _isRecharging = false;

    private void Update()
    {
        if (!_isded)
        {
            if (!_isRecharging)
            {
                _oxygenLevel = _oxygenLevel - (_oxygenConsumptionRate * Time.deltaTime);

                if (_oxygenLevel <= 0)
                {
                    Death();
                }
            }
            else
            {
                _oxygenLevel = _oxygenLevel + (_oxygenRechargeRate * Time.deltaTime);

                if (_oxygenLevel > _maxOxygen)
                {
                    _oxygenLevel = _maxOxygen;
                }
            }

            UpdateUI();
        }
    }

    private void Death()
    {
        _isded = true;
    }

    private void UpdateUI()
    {
        _uiPanel.localScale = new Vector3(1, _oxygenLevel / 100, 1);
    }

    public void ToggleRecharging(bool chargeState)
    {
        _isRecharging = chargeState;
    }

    public void KillPlayer()
    {
        Death();
    }
}
