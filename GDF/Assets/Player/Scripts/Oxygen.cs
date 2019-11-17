using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

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
    [SerializeField]
    private GameObject _deathUI;
    [SerializeField]
    private PostProcessProfile profile;

    private List<float> inputIntensities = new List<float>();
    private bool _isRecharging = false;

    private void Start()
    {
        _deathUI.SetActive(false);
    }

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

    private void LateUpdate()
    {
        ApplyIntensities();
    }

    private void Death()
    {
        _isded = true;
        GetComponent<MoveController>().SetMovementEnabled(false);
        _deathUI.SetActive(true);
        profile.GetSetting<Grain>().intensity.value = 1;
    }

    private void UpdateUI()
    {
        _uiPanel.localScale = new Vector3(1, _oxygenLevel / 100, 1);
    }

    private void ApplyIntensities()
    {
        float total = 0;
        float applyPercentages = 0.1f;

        if (inputIntensities.Count > 0)
        {

            foreach (float f in inputIntensities)
            {
                total += f;
            }

            applyPercentages = total / inputIntensities.Count;

            if (applyPercentages == 0)
            {
                applyPercentages = 0.1f;
            }
        }

        profile.GetSetting<Grain>().intensity.value = applyPercentages;
        profile.GetSetting<Vignette>().intensity.value = applyPercentages;

        inputIntensities = new List<float>();
    }

    public void ToggleRecharging(bool chargeState)
    {
        _isRecharging = chargeState;
    }

    public void KillPlayer()
    {
        Death();
    }

    public void DyingEffects(float intensity)
    {
        inputIntensities.Add(intensity);
    }
}
