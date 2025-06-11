using System;
using Core.Entity;
using Sirenix.OdinInspector;

public class DefineNameJokerCard
{
	public static string j_joker = "j_joker";
	public static string j_walkie_talkie = "j_walkie_talkie";
	public static string j_misprint = "j_misprint";
	public static string j_abstract_joker = "j_abstract_joker";
	public static string j_greedy_joker = "j_greedy_joker";
	public static string j_add_mult_by_suit = "j_add_mult_by_suit"; //use for Joker 2,3,4,5
	public static string j_add_mult_by_hand = "j_add_mult_by_hand"; //add mult by Poker hand
	public static string j_add_mult_by_rank = "j_add_mult_by_rank"; //add mult by card rank
	public static string j_add_chip_by_hand = "j_add_chip_by_hand"; //add chip by Poker hand
	public static string j_add_chip_by_rank = "j_add_chip_by_rank"; //add chip by card rank
	public static string j_add_chip_by_face_card = "j_add_chip_by_face_card"; //add chip by card rank
	public static string j_add_dollar_by_rank = "j_add_dollar_by_rank"; //add dollar by card
	public static string j_smiley_face = "j_smiley_face"; //add dollar by card
	public static string j_half_joker = "j_half_joker";
	public static string j_turtle_bean = "j_turtle_bean";
	public static string j_swashbuckler = "j_swashbuckler";
	public static string j_dusk = "j_dusk";
	public static string j_banner = "j_banner";
	public static string j_mystic_summit = "j_mystic_summit";
	public static string j_burglar = "j_burglar";
	public static string J_blue_joker = "J_blue_joker";
	public static string j_four_fingers = "j_four_fingers ";
	public static string j_chaos_the_clown = "j_chaos_the_clown";
	public static string j_pareidolia = "j_pareidolia";
	public static string j_trigger_on_hand = "j_trigger_on_hand";
	public static string j_cloud9 = "j_cloud9";
	public static string j_add_mult_by_hand_data = "j_add_mult_by_hand_data";
	public static string j_loyalty_card = "j_loyalty_card";
	public static string j_joker_stencil = "j_joker_stencil";
	public static string j_add_hand = "j_add_hand";
	public static string j_add_point = "j_add_point";
	public static string j_supernova = "j_supernova";
	public static string j_ride_the_bus = "j_ride_the_bus";
	public static string j_blackboard = "j_blackboard";
	public static string j_runner = "j_runner";
	public static string j_ice_cream = "j_ice_cream";
	public static string j_aegis = "j_aegis";
	public static string j_explosive_joker = "j_explosive_joker";
	public static string j_splash = "j_splash";
	public static string j_overwhelming_joker = "j_overwhelming_joker";
	public static string j_baseball_card = "j_baseball_card";
	public static string j_extra_lives = "j_extra_lives";
	public static string j_pairing_socks = "j_pairing_socks";
	public static string j_double_counting = "j_double_counting";
	public static string j_market_joker = "j_market_joker";
}
[Serializable]
public enum JokerContextEnum
{
	None = 0,
	HandPlayedData,
	ChangeHandSize,
	NumberOfJokerOwned,
	TriggerJoker,
	TriggerCardPlayed,
	TriggerCardInHand,
	RemainingDiscard,
	ChangeDiscardAmount,
	RemainingCardInDeck,
	AllCardInFullDeck,
	NumberCardTypeInDeck,
	NumberCardTypeInHand,
	CardsAreConsideredAs,
	PokerHandLevelAndHandType,
	PokerHandRequirement,
	EnemyData,
	EnemyDeckData,
	NumberOfHandPlayed,
	AddAnyCard,
	AddAnyJoker,
	DestroyJoker,
	////ChangeCardTypeToCardType,
	//CurrentMoneyData,
	//ModifyMoneyData,
	//TriggerOnSell,
	ShopModify,
	CardInHandData,
}

public enum JokerActivationEnum
{
	None,
	OnScore,
	Passive,
	Independent,
	OnTrigger,
	TriggerOnHand,
	AfterScore,
	AfterDealDame,
	EndGame

}

// vfx for joker
public enum EffectEnum
{
	None,
	Mult,
	RankMult,
	SuitMult,
}

[Serializable]
public class TypeEffectModel
{
	public TypeEffectEnum Type;
	[ShowIf("ShouldShowxMultValue")]
	public float xMultValue;
	[HideIf("ShouldShowxMultValue")]
	public int Value;

	[ShowIf("ShouldShowSuitAndRank")]
	public SuitCard SuitCard;
	[ShowIf("ShouldShowSuitAndRank")]
	public RankCard RankCard;
	[ShowIf("ShouldShowSuitAndRank")]
	public bool IsFaceCard;
	[ShowIf("ShouldShowSuitAndRank")]
	public PokerHandValue PokerHandValue;



	private bool ShouldShowSuitAndRank => Type == TypeEffectEnum.Chip || Type == TypeEffectEnum.AddMult || Type == TypeEffectEnum.Dollar;
	private bool ShouldShowxMultValue => Type == TypeEffectEnum.MultiplyMult;
}

public enum TypeEffectEnum
{
	None,
	Chip,
	AddMult,
	MultiplyMult,
	HandCount,
	Trigger,
	Normal,
	Dollar,
	Heal,
}
public enum RarityEnum
{
	None = 0,
	Uncommon = 1,
	Common,
	Rare,
	Legend
}