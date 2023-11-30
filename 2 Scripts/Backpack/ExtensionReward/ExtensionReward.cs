using BackpackSurvivors.Backpack;
using BackpackSurvivors.Shared;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BackpackSurvivors.Shared.Enums;

public class ExtensionReward : MonoBehaviour
{
    [SerializeField] BackpackExtension _backpackExtension;
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] GameObject _passiveContainer;
    [SerializeField] GameObject _extensionContainer;
    [SerializeField] NegativePassivePrefab _negativePassivePrefab;
    [SerializeField] PositivePassivePrefab _positivePassivePrefab;
    public List<GeneratedPassive> NegativePassives;
    public List<GeneratedPassive> PositivePassives;
    BackPackController _backpackGridManager;
    [SerializeField] TextMeshProUGUI _positiveText;
    [SerializeField] TextMeshProUGUI _negativeText;
    public BackpackExtension Extension;

    public int positiveWeight;
    public int negativeWeight;

    public void Init(
        BackpackExtension backpackExtension,
        List<GeneratedPassive> negativePassives,
        List<GeneratedPassive> positivePassives,
        BackPackController backpackGridManager,
        Canvas canvas)
    {
        _backpackExtension = backpackExtension;
        NegativePassives = negativePassives;
        PositivePassives = positivePassives;
        _backpackGridManager = backpackGridManager;

        Extension = _backpackGridManager.CreateExtensionItem(backpackExtension, transform, true, true);

        _titleText.SetText(string.Format("{0} bag extension", backpackExtension.ExtensionName));

        backpackGridManager.ResolveRewardPosition(Extension, _extensionContainer.transform);


        foreach (var negativePassive in NegativePassives)
        {
            var negativePassiveText = Instantiate(_negativePassivePrefab, _passiveContainer.transform);
            //Debug.Log($"Negative Passive generated: {negativePassive.PassiveType} {negativePassive.PassiveValue} {negativePassive.StatTarget}");
            negativePassiveText.SetText(backpackGridManager.ItemDatabase.GetCleanString(negativePassive.PassiveType, negativePassive.PassiveValue, negativePassive.StatTarget));
        }

        foreach (var positivePassive in PositivePassives)
        {
            var positivePassiveText = Instantiate(_positivePassivePrefab, _passiveContainer.transform);
            //Debug.Log($"Positive Passive generated: {positivePassive.PassiveType} {positivePassive.PassiveValue} {positivePassive.StatTarget}");
            positivePassiveText.SetText(backpackGridManager.ItemDatabase.GetCleanString(positivePassive.PassiveType, positivePassive.PassiveValue, positivePassive.StatTarget));
        }

        // TODO: Get weights for this
        positiveWeight = 3;
        _positiveText.SetText(positiveWeight.ToString());

        negativeWeight = 2;
        _negativeText.SetText(negativeWeight.ToString());
    }

    public void SelectReward()
    {
        _backpackGridManager.SelectReward(this);
    }

    private void Update()
    {
        if (Extension != null && Extension.Locked)
        {
            ((RectTransform)Extension.transform).localPosition = new Vector3(0, 0, 0);
        }
    }

}

