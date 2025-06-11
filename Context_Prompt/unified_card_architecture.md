# Unified Card Architecture - Kiến trúc Card System Tối ưu

## 🎯 Phân tích vấn đề hiện tại

Từ source code của bạn, tôi nhận thấy card system hiện tại có những vấn đề nghiêm trọng sau:

### **Vấn đề chính:**

**1. Lack of Unified Base Class:** Mỗi loại card (Poker, Joker, Tarot) có structure riêng biệt hoàn toàn, không share common behaviors.

**2. Hard-coded Logic:** Poker hand detection được hard-code trong `PlayCardView` với rất nhiều if-else statements, rất khó maintain.

**3. Tight Coupling:** Card effects trực tiếp modify game state thay vì thông qua clean interfaces.

**4. No Composition:** Card enhancements (seals, editions, modifications) không có cách nào để compose được với nhau.

**5. Mixed Responsibilities:** Card presentation logic bị mix với game logic.

### **Tại sao điều này quan trọng:**

Hãy nghĩ về card system như một thư viện. Bạn có nhiều loại sách (Poker cards), tạp chí (Joker cards), và báo (Tarot cards). Hiện tại, bạn đang quản lý từng loại với một hệ thống hoàn toàn khác nhau. Điều này có nghĩa là khi bạn muốn thêm một loại xuất bản mới (special cards), bạn phải tạo ra một hệ thống hoàn toàn mới thay vì mở rộng từ cái đã có.

## 🏗️ Kiến trúc Unified Card System mới

### **Core Philosophy: "Everything is a Card"**

Ý tưởng chính là mọi thứ trong game đều là "Card" với các behaviors khác nhau. Một Poker card có thể play được, một Joker card có effect, một Tarot card có special power. Nhưng tất cả đều share common interfaces và behaviors.

### **Cấu trúc Layer mới:**

```
📁 Cards/
├── 📁 Core/                    # Core abstractions
│   ├── ICard.cs               # Base interface for all cards
│   ├── CardBase.cs            # Abstract base implementation
│   └── CardIdentity.cs        # Unique identification system
├── 📁 Types/                   # Concrete card types
│   ├── PokerCard.cs           # Traditional playing cards
│   ├── JokerCard.cs           # Effect cards
│   ├── TarotCard.cs           # Special power cards
│   └── SpellCard.cs           # Consumable cards
├── 📁 Components/              # Card component system
│   ├── ICardComponent.cs      # Component interface
│   ├── PlayableComponent.cs   # Can be played in hands
│   ├── EffectComponent.cs     # Has triggered effects
│   └── ModifierComponent.cs   # Modifies other cards
├── 📁 Modifiers/              # Card modification system
│   ├── ICardModifier.cs       # Modifier interface
│   ├── EnhancementModifier.cs # Bonus, Mult, Wild, etc.
│   ├── EditionModifier.cs     # Foil, Holographic, etc.
│   └── SealModifier.cs        # Gold, Red, Blue seals
├── 📁 Behaviors/              # Card behavior system
│   ├── ICardBehavior.cs       # Behavior interface
│   ├── ScoreBehavior.cs       # Scoring contributions
│   ├── TriggerBehavior.cs     # When/how effects trigger
│   └── InteractionBehavior.cs # How cards interact
├── 📁 Factory/                # Card creation system
│   ├── ICardFactory.cs        # Factory interface
│   ├── CardBuilder.cs         # Fluent builder pattern
│   └── CardPool.cs            # Card pool management
└── 📁 Evaluation/             # Hand evaluation system
    ├── IHandEvaluator.cs      # Evaluation interface
    ├── PokerHandEvaluator.cs  # Poker hand logic
    └── ComboEvaluator.cs      # Multi-card combinations
```

## 🔧 Core Implementation

### **1. Unified Card Interface & Base Class**

```csharp
// Cards/Core/ICard.cs - The foundation of everything
public interface ICard
{
    CardIdentity Identity { get; }           // Unique identification
    string Name { get; }                     // Display name
    string Description { get; }              // Card description
    Rarity Rarity { get; }                  // Card rarity
    CardState State { get; }                // Current state (in hand, in deck, etc.)
    
    // Component system - this is the key to flexibility
    T GetComponent<T>() where T : class, ICardComponent;
    void AddComponent(ICardComponent component);
    void RemoveComponent<T>() where T : class, ICardComponent;
    bool HasComponent<T>() where T : class, ICardComponent;
    
    // Modifier system - for enhancements, editions, seals
    void ApplyModifier(ICardModifier modifier);
    void RemoveModifier(ICardModifier modifier);
    IReadOnlyList<ICardModifier> GetModifiers<T>() where T : ICardModifier;
    
    // Event system for card interactions
    event Action<ICard, CardEvent> OnCardEvent;
    
    // Utility methods
    ICard Clone();                          // Create copy
    bool CanInteractWith(ICard other);      // Interaction rules
}

// Cards/Core/CardBase.cs - Common implementation
public abstract class CardBase : ICard
{
    private readonly List<ICardComponent> _components;
    private readonly List<ICardModifier> _modifiers;
    
    public CardIdentity Identity { get; private set; }
    public virtual string Name { get; protected set; }
    public virtual string Description { get; protected set; }
    public Rarity Rarity { get; protected set; }
    public CardState State { get; private set; }
    
    public event Action<ICard, CardEvent> OnCardEvent;
    
    protected CardBase(CardIdentity identity, string name, Rarity rarity)
    {
        Identity = identity;
        Name = name;
        Rarity = rarity;
        _components = new List<ICardComponent>();
        _modifiers = new List<ICardModifier>();
    }
    
    // Component system implementation
    public T GetComponent<T>() where T : class, ICardComponent
    {
        return _components.OfType<T>().FirstOrDefault();
    }
    
    public void AddComponent(ICardComponent component)
    {
        if (component == null) return;
        
        // Ensure only one component of each type
        RemoveComponent(component.GetType());
        _components.Add(component);
        component.OnAttached(this);
    }
    
    // Modifier system - this allows for complex card modifications
    public void ApplyModifier(ICardModifier modifier)
    {
        if (modifier == null || _modifiers.Contains(modifier)) return;
        
        _modifiers.Add(modifier);
        modifier.OnApplied(this);
        
        // Recalculate all derived properties
        RecalculateProperties();
        TriggerEvent(new CardModifiedEvent(this, modifier));
    }
    
    // This is where the magic happens - derived properties are recalculated
    // whenever modifiers change
    protected virtual void RecalculateProperties()
    {
        // Base implementation updates description based on modifiers
        var modifiedDescription = GetBaseDescription();
        
        foreach (var modifier in _modifiers)
        {
            modifiedDescription = modifier.ModifyDescription(modifiedDescription);
        }
        
        Description = modifiedDescription;
    }
    
    protected abstract string GetBaseDescription();
    
    protected void TriggerEvent(CardEvent cardEvent)
    {
        OnCardEvent?.Invoke(this, cardEvent);
    }
}
```

### **2. Component System - The Heart of Flexibility**

```csharp
// Cards/Components/ICardComponent.cs
public interface ICardComponent
{
    string ComponentType { get; }
    void OnAttached(ICard card);
    void OnDetached(ICard card);
    bool IsCompatibleWith(ICard card);
}

// Cards/Components/PlayableComponent.cs - For cards that can be played
public class PlayableComponent : ICardComponent
{
    public string ComponentType => "Playable";
    
    public Rank Rank { get; private set; }
    public Suit Suit { get; private set; }
    public int BaseChips { get; private set; }
    public int BaseMultiplier { get; private set; }
    
    // These get recalculated when modifiers change
    public int EffectiveChips { get; private set; }
    public int EffectiveMultiplier { get; private set; }
    
    private ICard _owner;
    
    public PlayableComponent(Rank rank, Suit suit, int baseChips, int baseMultiplier)
    {
        Rank = rank;
        Suit = suit;
        BaseChips = baseChips;
        BaseMultiplier = baseMultiplier;
    }
    
    public void OnAttached(ICard card)
    {
        _owner = card;
        RecalculateEffectiveValues();
        
        // Subscribe to modifier changes
        card.OnCardEvent += OnCardEvent;
    }
    
    public void OnDetached(ICard card)
    {
        card.OnCardEvent -= OnCardEvent;
        _owner = null;
    }
    
    private void OnCardEvent(ICard card, CardEvent cardEvent)
    {
        if (cardEvent is CardModifiedEvent)
        {
            RecalculateEffectiveValues();
        }
    }
    
    // This method shows how modifiers affect card properties
    private void RecalculateEffectiveValues()
    {
        if (_owner == null) return;
        
        EffectiveChips = BaseChips;
        EffectiveMultiplier = BaseMultiplier;
        
        // Apply all relevant modifiers
        foreach (var modifier in _owner.GetModifiers<IStatModifier>())
        {
            EffectiveChips = modifier.ModifyChips(EffectiveChips);
            EffectiveMultiplier = modifier.ModifyMultiplier(EffectiveMultiplier);
        }
    }
    
    public bool IsCompatibleWith(ICard card) => true; // Most cards can be playable
}

// Cards/Components/EffectComponent.cs - For cards with triggered effects
public class EffectComponent : ICardComponent
{
    public string ComponentType => "Effect";
    
    private readonly List<ICardEffect> _effects;
    private ICard _owner;
    
    public EffectComponent()
    {
        _effects = new List<ICardEffect>();
    }
    
    public void AddEffect(ICardEffect effect)
    {
        if (effect != null && !_effects.Contains(effect))
        {
            _effects.Add(effect);
        }
    }
    
    // This is how Joker effects get triggered
    public async Task<EffectResult> TriggerEffects(EffectContext context)
    {
        var results = new List<EffectResult>();
        
        foreach (var effect in _effects.Where(e => e.CanTrigger(context)))
        {
            var result = await effect.Execute(context);
            results.Add(result);
            
            // Early exit if effect says to stop processing
            if (result.StopProcessing)
                break;
        }
        
        return EffectResult.Combine(results);
    }
    
    public void OnAttached(ICard card)
    {
        _owner = card;
    }
    
    public void OnDetached(ICard card)
    {
        _owner = null;
    }
    
    public bool IsCompatibleWith(ICard card) => true;
}
```

### **3. Modifier System - For Enhancements, Editions, Seals**

```csharp
// Cards/Modifiers/ICardModifier.cs
public interface ICardModifier
{
    string ModifierType { get; }
    string Name { get; }
    int Priority { get; }                    // For ordering modifier application
    
    void OnApplied(ICard card);
    void OnRemoved(ICard card);
    string ModifyDescription(string baseDescription);
    
    // Modifiers can affect different aspects of cards
    bool AffectsStats { get; }
    bool AffectsAppearance { get; }
    bool AffectsBehavior { get; }
}

// Cards/Modifiers/EnhancementModifier.cs
public class EnhancementModifier : ICardModifier, IStatModifier
{
    public string ModifierType => "Enhancement";
    public string Name { get; private set; }
    public int Priority => 100;             // High priority for base stats
    
    public bool AffectsStats => true;
    public bool AffectsAppearance => true;
    public bool AffectsBehavior => false;
    
    private readonly EnhancementType _enhancementType;
    private readonly int _chipBonus;
    private readonly int _multiplierBonus;
    private readonly bool _isWild;
    
    public EnhancementModifier(EnhancementType type, int chipBonus = 0, int multiplierBonus = 0, bool isWild = false)
    {
        _enhancementType = type;
        _chipBonus = chipBonus;
        _multiplierBonus = multiplierBonus;
        _isWild = isWild;
        Name = type.ToString();
    }
    
    public int ModifyChips(int baseChips)
    {
        return _enhancementType switch
        {
            EnhancementType.Bonus => baseChips + _chipBonus,
            EnhancementType.Steel => baseChips * 2,        // Steel cards get 2x chips
            EnhancementType.Stone => baseChips + 50,       // Stone cards get +50 chips
            _ => baseChips
        };
    }
    
    public int ModifyMultiplier(int baseMultiplier)
    {
        return _enhancementType switch
        {
            EnhancementType.Mult => baseMultiplier + _multiplierBonus,
            EnhancementType.Wild => baseMultiplier * 2,    // Wild cards might get mult bonus
            _ => baseMultiplier
        };
    }
    
    public string ModifyDescription(string baseDescription)
    {
        return _enhancementType switch
        {
            EnhancementType.Wild => $"{baseDescription} (Wild - counts as any rank/suit)",
            EnhancementType.Glass => $"{baseDescription} (Glass - 2x mult, but fragile)",
            EnhancementType.Steel => $"{baseDescription} (Steel - 2x chips)",
            _ => $"{baseDescription} ({Name})"
        };
    }
    
    public void OnApplied(ICard card) { }
    public void OnRemoved(ICard card) { }
}

// Cards/Modifiers/SealModifier.cs - Example of behavior-affecting modifier
public class SealModifier : ICardModifier
{
    public string ModifierType => "Seal";
    public string Name { get; private set; }
    public int Priority => 50;              // Medium priority
    
    public bool AffectsStats => false;
    public bool AffectsAppearance => true;
    public bool AffectsBehavior => true;    // Seals change when/how effects trigger
    
    private readonly SealType _sealType;
    
    public SealModifier(SealType sealType)
    {
        _sealType = sealType;
        Name = sealType.ToString();
    }
    
    public string ModifyDescription(string baseDescription)
    {
        var sealDescription = _sealType switch
        {
            SealType.Gold => "Creates $3 when played",
            SealType.Red => "Retriggers this card",
            SealType.Blue => "Creates Planet card when last card in hand",
            SealType.Purple => "Creates Tarot card when discarded",
            _ => ""
        };
        
        return $"{baseDescription}\n[{Name} Seal: {sealDescription}]";
    }
    
    // This is where seal behavior gets implemented
    public void OnApplied(ICard card)
    {
        // Add appropriate effect component based on seal type
        var effectComponent = card.GetComponent<EffectComponent>();
        if (effectComponent == null)
        {
            effectComponent = new EffectComponent();
            card.AddComponent(effectComponent);
        }
        
        ICardEffect sealEffect = _sealType switch
        {
            SealType.Gold => new GoldSealEffect(),
            SealType.Red => new RedSealEffect(),
            SealType.Blue => new BlueSealEffect(),
            SealType.Purple => new PurpleSealEffect(),
            _ => null
        };
        
        if (sealEffect != null)
        {
            effectComponent.AddEffect(sealEffect);
        }
    }
    
    public void OnRemoved(ICard card) { }
}
```

### **4. Concrete Card Types**

```csharp
// Cards/Types/PokerCard.cs - Traditional playing cards
public class PokerCard : CardBase
{
    public PokerCard(Rank rank, Suit suit) 
        : base(CardIdentity.Generate($"poker_{rank}_{suit}"), GetCardName(rank, suit), Rarity.Common)
    {
        // Every poker card is playable by default
        var playableComponent = new PlayableComponent(rank, suit, GetBaseChips(rank), GetBaseMultiplier(rank));
        AddComponent(playableComponent);
    }
    
    private static string GetCardName(Rank rank, Suit suit)
    {
        return $"{rank} of {suit}";
    }
    
    private static int GetBaseChips(Rank rank)
    {
        // Use configuration system for actual values
        return ConfigManager.Instance.GetCardChips(rank);
    }
    
    private static int GetBaseMultiplier(Rank rank)
    {
        return ConfigManager.Instance.GetCardMultiplier(rank);
    }
    
    protected override string GetBaseDescription()
    {
        var playable = GetComponent<PlayableComponent>();
        return $"{playable.Rank} of {playable.Suit} - {playable.EffectiveChips} chips, {playable.EffectiveMultiplier} mult";
    }
    
    // Poker cards can have enhancements applied
    public void ApplyEnhancement(EnhancementType enhancement)
    {
        var modifier = new EnhancementModifier(enhancement);
        ApplyModifier(modifier);
    }
    
    public void ApplySeal(SealType seal)
    {
        var modifier = new SealModifier(seal);
        ApplyModifier(modifier);
    }
}

// Cards/Types/JokerCard.cs - Effect cards
public class JokerCard : CardBase
{
    private readonly JokerDefinition _definition;
    
    public JokerCard(JokerDefinition definition) 
        : base(CardIdentity.Generate($"joker_{definition.EffectName}"), definition.Name, definition.Rarity)
    {
        _definition = definition;
        
        // Every joker has an effect component
        var effectComponent = new EffectComponent();
        AddComponent(effectComponent);
        
        // Create the specific effect based on definition
        var jokerEffect = JokerEffectFactory.CreateEffect(definition);
        effectComponent.AddEffect(jokerEffect);
    }
    
    protected override string GetBaseDescription()
    {
        var description = _definition.BaseDescription;
        
        // Add dynamic values if any
        if (_definition.HasDynamicValues)
        {
            description = _definition.FormatDescription(GetDynamicValues());
        }
        
        return description;
    }
    
    private Dictionary<string, object> GetDynamicValues()
    {
        // This is where jokers like "Stencil" get their dynamic values
        // "Gives X2 Mult per empty Joker slot"
        return new Dictionary<string, object>
        {
            ["emptySlots"] = GetEmptyJokerSlots(),
            ["ownedJokers"] = GetOwnedJokersCount(),
            ["handsPlayed"] = GetHandsPlayedThisRun()
        };
    }
    
    private int GetEmptyJokerSlots()
    {
        // Get from player service
        return ServiceLocator.Get<IPlayerService>().GetEmptyJokerSlots();
    }
}

// Cards/Types/TarotCard.cs - Special power cards
public class TarotCard : CardBase
{
    private readonly TarotDefinition _definition;
    private bool _isUsed;
    
    public TarotCard(TarotDefinition definition) 
        : base(CardIdentity.Generate($"tarot_{definition.Name}"), definition.Name, definition.Rarity)
    {
        _definition = definition;
        
        // Tarot cards have one-time effects
        var effectComponent = new EffectComponent();
        AddComponent(effectComponent);
        
        var tarotEffect = TarotEffectFactory.CreateEffect(definition);
        effectComponent.AddEffect(tarotEffect);
    }
    
    protected override string GetBaseDescription()
    {
        var description = _definition.Description;
        
        if (_isUsed)
        {
            description += "\n(Used)";
        }
        
        return description;
    }
    
    public async Task<bool> UseCard(EffectContext context)
    {
        if (_isUsed) return false;
        
        var effectComponent = GetComponent<EffectComponent>();
        var result = await effectComponent.TriggerEffects(context);
        
        if (result.IsSuccess)
        {
            _isUsed = true;
            TriggerEvent(new CardUsedEvent(this));
            return true;
        }
        
        return false;
    }
}
```

### **5. Builder Pattern for Card Creation**

```csharp
// Cards/Factory/CardBuilder.cs - Fluent interface for creating complex cards
public class CardBuilder
{
    private CardType _cardType;
    private Rank _rank;
    private Suit _suit;
    private string _name;
    private Rarity _rarity = Rarity.Common;
    private readonly List<ICardModifier> _modifiers = new();
    private readonly List<ICardEffect> _effects = new();
    
    public static CardBuilder Create() => new();
    
    public CardBuilder PokerCard(Rank rank, Suit suit)
    {
        _cardType = CardType.Poker;
        _rank = rank;
        _suit = suit;
        return this;
    }
    
    public CardBuilder JokerCard(string effectName)
    {
        _cardType = CardType.Joker;
        _name = effectName;
        return this;
    }
    
    public CardBuilder WithEnhancement(EnhancementType enhancement)
    {
        _modifiers.Add(new EnhancementModifier(enhancement));
        return this;
    }
    
    public CardBuilder WithSeal(SealType seal)
    {
        _modifiers.Add(new SealModifier(seal));
        return this;
    }
    
    public CardBuilder WithEdition(EditionType edition)
    {
        _modifiers.Add(new EditionModifier(edition));
        return this;
    }
    
    public CardBuilder WithRarity(Rarity rarity)
    {
        _rarity = rarity;
        return this;
    }
    
    public ICard Build()
    {
        ICard card = _cardType switch
        {
            CardType.Poker => new PokerCard(_rank, _suit),
            CardType.Joker => CreateJokerCard(_name),
            CardType.Tarot => CreateTarotCard(_name),
            _ => throw new ArgumentException($"Unknown card type: {_cardType}")
        };
        
        // Apply all modifiers
        foreach (var modifier in _modifiers)
        {
            card.ApplyModifier(modifier);
        }
        
        return card;
    }
    
    // Example usage:
    // var card = CardBuilder.Create()
    //     .PokerCard(Rank.Ace, Suit.Spades)
    //     .WithEnhancement(EnhancementType.Gold)
    //     .WithSeal(SealType.Red)
    //     .WithEdition(EditionType.Foil)
    //     .Build();
}
```

### **6. Hand Evaluation System Refactor**

```csharp
// Cards/Evaluation/IHandEvaluator.cs
public interface IHandEvaluator
{
    HandEvaluationResult EvaluateHand(IReadOnlyList<ICard> cards);
    bool CanEvaluate(IReadOnlyList<ICard> cards);
    int Priority { get; }                   // Higher priority evaluated first
}

// Cards/Evaluation/PokerHandEvaluator.cs - Clean poker logic
public class PokerHandEvaluator : IHandEvaluator
{
    public int Priority => 100;
    
    private readonly Dictionary<PokerHandType, IPokerHandPattern> _patterns;
    
    public PokerHandEvaluator()
    {
        _patterns = new Dictionary<PokerHandType, IPokerHandPattern>
        {
            [PokerHandType.RoyalFlush] = new RoyalFlushPattern(),
            [PokerHandType.StraightFlush] = new StraightFlushPattern(),
            [PokerHandType.FourOfAKind] = new FourOfAKindPattern(),
            [PokerHandType.FullHouse] = new FullHousePattern(),
            [PokerHandType.Flush] = new FlushPattern(),
            [PokerHandType.Straight] = new StraightPattern(),
            [PokerHandType.ThreeOfAKind] = new ThreeOfAKindPattern(),
            [PokerHandType.TwoPair] = new TwoPairPattern(),
            [PokerHandType.Pair] = new PairPattern(),
            [PokerHandType.HighCard] = new HighCardPattern()
        };
    }
    
    public bool CanEvaluate(IReadOnlyList<ICard> cards)
    {
        // Can evaluate if all cards are playable poker cards
        return cards.All(c => c.HasComponent<PlayableComponent>());
    }
    
    public HandEvaluationResult EvaluateHand(IReadOnlyList<ICard> cards)
    {
        if (!CanEvaluate(cards))
            return HandEvaluationResult.Invalid();
        
        var playableCards = cards.Select(c => c.GetComponent<PlayableComponent>()).ToList();
        
        // Check patterns in priority order (best hands first)
        foreach (var pattern in _patterns.Values.OrderByDescending(p => p.Priority))
        {
            if (pattern.Matches(playableCards))
            {
                var matchingCards = pattern.GetMatchingCards(playableCards);
                return new HandEvaluationResult(pattern.HandType, matchingCards, pattern.CalculateScore(matchingCards));
            }
        }
        
        return HandEvaluationResult.Invalid();
    }
}

// Cards/Evaluation/Patterns/IPokerHandPattern.cs
public interface IPokerHandPattern
{
    PokerHandType HandType { get; }
    int Priority { get; }
    bool Matches(IReadOnlyList<PlayableComponent> cards);
    IReadOnlyList<PlayableComponent> GetMatchingCards(IReadOnlyList<PlayableComponent> cards);
    HandScore CalculateScore(IReadOnlyList<PlayableComponent> matchingCards);
}

// Cards/Evaluation/Patterns/FlushPattern.cs - Example clean pattern implementation
public class FlushPattern : IPokerHandPattern
{
    public PokerHandType HandType => PokerHandType.Flush;
    public int Priority => 40;
    
    public bool Matches(IReadOnlyList<PlayableComponent> cards)
    {
        if (cards.Count < 5) return false;
        
        // Check for joker modifications that might change flush requirements
        var requiredCount = GetRequiredFlushCount();
        
        var suitGroups = cards.GroupBy(c => GetEffectiveSuit(c));
        return suitGroups.Any(g => g.Count() >= requiredCount);
    }
    
    public IReadOnlyList<PlayableComponent> GetMatchingCards(IReadOnlyList<PlayableComponent> cards)
    {
        var requiredCount = GetRequiredFlushCount();
        var suitGroups = cards.GroupBy(c => GetEffectiveSuit(c));
        var flushGroup = suitGroups.First(g => g.Count() >= requiredCount);
        
        return flushGroup.OrderByDescending(c => c.Rank).Take(5).ToList();
    }
    
    public HandScore CalculateScore(IReadOnlyList<PlayableComponent> matchingCards)
    {
        var baseChips = matchingCards.Sum(c => c.EffectiveChips);
        var baseMultiplier = matchingCards.Sum(c => c.EffectiveMultiplier);
        
        // Add flush bonus
        var flushBonus = ConfigManager.Instance.GetHandBonus(PokerHandType.Flush);
        
        return new HandScore(baseChips + flushBonus.Chips, baseMultiplier + flushBonus.Multiplier);
    }
    
    private int GetRequiredFlushCount()
    {
        // Check for "Four Fingers" joker that reduces flush requirement to 4
        var jokerService = ServiceLocator.Get<IJokerService>();
        return jokerService.HasActiveJoker("j_four_fingers") ? 4 : 5;
    }
    
    private Suit GetEffectiveSuit(PlayableComponent card)
    {
        // Check for wild cards that can count as any suit
        var owner = card.GetOwner();
        var wildModifiers = owner.GetModifiers<EnhancementModifier>()
            .Where(m => m.IsWild);
        
        if (wildModifiers.Any())
        {
            // Wild cards take the suit that would create the best flush
            return DetermineOptimalWildSuit(card, GetOtherCards());
        }
        
        return card.Suit;
    }
}
```

## 🎮 Usage Examples

### **Creating Cards with the New System:**

```csharp
// Simple poker card
var aceOfSpades = CardBuilder.Create()
    .PokerCard(Rank.Ace, Suit.Spades)
    .Build();

// Enhanced poker card
var goldenWildAce = CardBuilder.Create()
    .PokerCard(Rank.Ace, Suit.Spades)
    .WithEnhancement(EnhancementType.Wild)
    .WithEnhancement(EnhancementType.Gold)
    .WithSeal(SealType.Red)
    .WithEdition(EditionType.Holographic)
    .Build();

// Joker card
var abstractJoker = CardBuilder.Create()
    .JokerCard("j_abstract_joker")
    .WithRarity(Rarity.Uncommon)
    .Build();

// Creating cards from factory
var cardFactory = ServiceLocator.Get<ICardFactory>();
var randomJoker = cardFactory.CreateRandomJoker(Rarity.Rare);
var standardDeck = cardFactory.CreateStandardDeck();
```

### **Playing Cards:**

```csharp
// Hand evaluation with new system
var selectedCards = GetSelectedCards();
var handEvaluator = ServiceLocator.Get<IHandEvaluationService>();

var result = handEvaluator.EvaluateHand(selectedCards);
if (result.IsValid)
{
    Debug.Log($"Hand: {result.HandType}, Score: {result.Score}");
    
    // Trigger all card effects
    var effectService = ServiceLocator.Get<ICardEffectService>();
    await effectService.TriggerPlayEffects(selectedCards, result);
}
```

### **Dynamic Card Modification:**

```csharp
// Runtime modification example
var card = GetPlayerCard();

// Apply enhancement
card.ApplyModifier(new EnhancementModifier(EnhancementType.Steel));

// The card automatically recalculates its properties
var playable = card.GetComponent<PlayableComponent>();
Debug.Log($"New chips: {playable.EffectiveChips}"); // Will be doubled for Steel
```

## 🏆 Benefits of the New Architecture

### **1. True Extensibility:**
Adding a new card type (like Spell cards) only requires implementing the base interface. All existing systems (modifiers, effects, factory) work automatically.

### **2. Clean Separation of Concerns:**
Card data, card presentation, and card effects are completely separate. You can change how cards look without touching game logic.

### **3. Powerful Composition:**
Any card can have any combination of enhancements, editions, and seals. New combinations work automatically without additional code.

### **4. Maintainable Hand Logic:**
Poker hand detection is now clean, testable, and easy to extend. Adding new hand types or modifying existing ones is straightforward.

### **5. Event-Driven Effects:**
Card interactions happen through events, making the system loosely coupled and easy to debug.

### **6. Type Safety:**
Strong typing prevents many runtime errors and makes the code self-documenting.

## 🔄 Migration Strategy

### **Phase 1: Core Foundation** (1 week)
Implement the base interfaces and component system. Create CardBase and basic components.

### **Phase 2: Poker Cards** (1 week)  
Migrate existing poker cards to the new system. Implement PlayableComponent and basic modifiers.

### **Phase 3: Joker System** (2 weeks)
Refactor joker system to use EffectComponent. This is the most complex part.

### **Phase 4: Hand Evaluation** (1 week)
Replace existing hand detection with the new pattern-based system.

### **Phase 5: UI Integration** (1 week)
Update UI to work with the new card system. Implement proper event handling.

This architecture will make your card system incredibly flexible, maintainable, and extensible. Each card type becomes a natural extension of the core system rather than a separate implementation.