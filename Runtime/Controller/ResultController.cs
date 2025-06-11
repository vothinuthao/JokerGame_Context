using System.Collections.Generic;
using System.Linq;
using Core.Entity;
using Core.Manager;
using Core.Utils;
using Manager;
using UnityEngine;

public sealed class BonusResult
{
    //public int Amount;
    public string Description;
    public float Quantity;
    public ColorScoreEnum ColorAmount = ColorScoreEnum.None;
}
public class ResultController : Singleton<ResultController>
{
    private int _curentAnte = 0;
    private int _currentRound = 0;

    public int CurrentAnte => _curentAnte;
    public int CurrentRound => _currentRound;
    public void OnInit()
    {
        _curentAnte = PlayerDataManager.Instance.Property.Ante;
        _currentRound = PlayerDataManager.Instance.Property.Round;

    }
    public int CreditCalculationFormula()
    {
        var round = PlayerDataManager.Instance.Property.Round;
        var configRound = ConfigManager.configPlayRound.GetValueByRound(round);
        var baseStakeReward = configRound.baseStakeReward;
        var playHandCount = PlayerDataManager.Instance.Property.PlayerLastResult.CountHandLastGame;
        var bonus = InterestCalculationFormula();
        int maxHandForBonus = Mathf.Clamp(6 - playHandCount, 0, 6);
        int finalReward = baseStakeReward + (maxHandForBonus) + bonus;
        return finalReward;
    }
    public int InterestCalculationFormula()
    {
        var currentDollar = PlayerDataManager.Instance.Property.Dollar;
        var configCredit = ConfigManager.configCreditEndGame.Records;
        var getValueInterest = configCredit.LastOrDefault(x => x.milestone <= currentDollar);
        return getValueInterest?.interest ?? 0;
    }

    public List<BonusResult> GenerateListBonus()
    {
        List<BonusResult> listBonus = new List<BonusResult>();
        var round = PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1);
        var configRound = ConfigManager.configPlayRound.GetValueByRound(round - 1);
        BonusResult bonus4 = new BonusResult()
        {
            //Amount = configRound.baseStakeReward,
            Description = "Defeat all enemies",
            Quantity = configRound.baseStakeReward,
            ColorAmount = ColorScoreEnum.DollarColor
        };
        listBonus.Add(bonus4);
        // used hand bonus 
        BonusResult bonus1 = new BonusResult()
        {
            //Amount = PlayerDataManager.Instance.Property.PlayerLastResult.CountHandLastGame,
            Description = "Efficiency bonus (6$, -1$ each hand played)",
            Quantity = Mathf.Clamp(6 - PlayerDataManager.Instance.Property.PlayerLastResult.CountHandLastGame, 0, 6),
            ColorAmount = ColorScoreEnum.HandColor
        };
        listBonus.Add(bonus1);
        // BonusResult bonus2 = new BonusResult()
        // {
        //     Amount = PlayCardController.Instance.Discards,
        //     Description = "Remaing Discard (-$1 Each)",
        //     Dollar = Mathf.Clamp(6 - PlayCardController.Instance.Hands, 0, 6)
        // };
        // used interests bonus 
        var currentDollar = PlayerDataManager.Instance.Property.Dollar;
        var config = ConfigManager.configCreditEndGame.Records;
        var getMax = config.LastOrDefault();
        var getPer = ConfigManager.configCreditEndGame.GetValueByID(2);
        var getValueInterest = config.LastOrDefault(x => x.milestone <= currentDollar);
        if (getMax != null && getValueInterest != null)
        {
            BonusResult bonus3 = new BonusResult()
            {
                //Amount = Mathf.Clamp(getValueInterest.id - 1, 0, config.Count),
                Description = $"Interests per ${getPer.milestone} (${getMax.interest} Max)",
                Quantity = InterestCalculationFormula(),
                ColorAmount = ColorScoreEnum.DollarColor
            };
            listBonus.Add(bonus3);
        }
        
       

        return listBonus;
    }
    
}