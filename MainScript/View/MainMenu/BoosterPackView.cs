using System;
using System.Collections.Generic;
using Core.Manager;
using Core.Manager.Configs;
using Core.Utils;
using InGame;
using Manager;
using Runtime.Manager;
using UnityEngine;

public class BoosterPackView : MonoBehaviour
{
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _prefabBoosterPack;

    private int _idPackSelected;
    private ShopController _shopController;
    private Action<BoosterPackVo> _callback;
    private void Start()
    {
        _prefabBoosterPack.SetActive(false);
    }

    public void OnInitPack(Action<BoosterPackVo> callback)
    {
        _callback = callback;
        List<ConfigBoosterPackRecord> listBoosterPack = new List<ConfigBoosterPackRecord>();
        if (PlayerDataManager.Instance.ShopProperty.BoosterPackOnShop.Count != 0)
        {
            foreach (var id in PlayerDataManager.Instance.ShopProperty.BoosterPackOnShop)
            {
                var config = ConfigManager.configBoosterPack.GetPackByID(id);
                listBoosterPack.Add(config);
            }
        }
        else
        {
            listBoosterPack = ShopController.Instance.RandomBoosterPackHaveAmount(2);
        }
        GameObjectUtils.Instance.ClearAllChild(_content.gameObject);
        for (var i = 0; i < listBoosterPack.Count; i++)
        {
            var pack = listBoosterPack[i];
            GameObject obj = GameObjectUtils.Instance.SpawnGameObject(_content, _prefabBoosterPack);
            BoosterPackVo script = obj.GetComponent<BoosterPackVo>();
            script.SetData(pack);
            script.OnInit(OnUpdate);
            obj.SetActive(true);
            ShopController.Instance.AddBoosterPackVo(script);
            PlayerDataManager.Instance.ShopProperty.SaveIdBoosterPackShop(pack.id);
            obj.name = "Booster Pack: " + i;
        }
    }
    
    private void OnUpdate(BoosterPackVo data)
    {
        _idPackSelected = data.Data.id;
        ShopController.Instance.SelectBoosterPack(data.Data.id);
        _callback?.Invoke(data);
    }
}
