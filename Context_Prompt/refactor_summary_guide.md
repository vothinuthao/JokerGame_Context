# Poker Game Architecture Transformation Guide

## 🎯 Core Objective

Transform monolithic Unity game architecture into clean, layered, testable system while preserving exact gameplay behavior.

## 📊 Current vs Target Architecture

### Current State (Problems)
```
┌─────────────────────────────────────┐
│           MonoBehaviour Layer       │
│  PlayCardView, GamePlayView, etc.   │
│  ├── Direct singleton access        │
│  ├── Mixed UI + business logic      │
│  ├── Hard-coded dependencies        │
│  └── Untestable code                │
└─────────────────────────────────────┘
┌─────────────────────────────────────┐
│         Singleton Managers          │
│ AudioManager, ConfigManager, etc.   │
│  ├── Global state                   │
│  ├── Hidden dependencies            │
│  └── Difficult to mock              │
└─────────────────────────────────────┘
```

### Target State (Solution)
```
┌─────────────────────────────────────┐
│         Presentation Layer          │
│    Views + ViewModels + Commands    │
│         (UI logic only)             │
└─────────────────────────────────────┘
           ↓ (interfaces)
┌─────────────────────────────────────┐
│        Application Layer            │
│     Services + Use Cases            │
│      (orchestration logic)          │
└─────────────────────────────────────┘
           ↓ (interfaces)
┌─────────────────────────────────────┐
│          Domain Layer               │
│    Entities + Business Rules        │
│       (core game logic)             │
└─────────────────────────────────────┘
           ↓ (interfaces)
┌─────────────────────────────────────┐
│       Infrastructure Layer          │
│   Data + Audio + Configuration      │
│       (external concerns)           │
└─────────────────────────────────────┘
```

## 🔧 Core Transformation Patterns

### 1. Singleton to Dependency Injection

**Before: Hard Dependencies**
```csharp
public class PlayCardView : MonoBehaviour
{
    private void PlaySound()
    {
        // Hard dependency - cannot test or mock
        AudioManager.Instance.PlaySFX(AudioName.Card1);
        var config = ConfigManager.Instance.GetCardConfig();
        var playerData = PlayerDataManager.Instance.Property;
    }
}
```

**After: Injected Dependencies**
```csharp
public class PlayCardView : MonoBehaviour
{
    private readonly IAudioService _audioService;
    private readonly IConfigService _configService;
    private readonly IPlayerService _playerService;
    
    // Dependencies injected by DI container
    [Inject]
    public void Initialize(IAudioService audioService, IConfigService configService, IPlayerService playerService)
    {
        _audioService = audioService;
        _configService = configService;
        _playerService = playerService;
    }
    
    private void PlaySound()
    {
        // Testable - can inject mocks
        _audioService.PlaySFX(AudioName.Card1);
        var config = _configService.GetCardConfig();
        var playerData = _playerService.GetPlayerData();
    }
}
```

### 2. Mixed Responsibilities to MVVM

**Before: Everything in View**
```csharp
public class PlayCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button playButton;
    
    private void Start()
    {
        playButton.onClick.AddListener(OnPlayCard);
        UpdateUI();
    }
    
    private void OnPlayCard()
    {
        // UI logic mixed with business logic
        var selectedCards = GetSelectedCards();
        var handType = EvaluateHand(selectedCards); // Business logic in UI!
        var score = CalculateScore(handType);       // Business logic in UI!
        
        scoreText.text = score.ToString();          // UI logic
        PlaySoundEffect();                          // UI logic
        
        PlayerDataManager.Instance.AddScore(score); // Data access in UI!
    }
    
    // Hundreds of lines of mixed logic...
}
```

**After: Separated Concerns**
```csharp
// ViewModel - Pure C# class (testable)
public class GamePlayViewModel : BaseViewModel
{
    private readonly IGameService _gameService;
    private readonly IHandEvaluator _handEvaluator;
    
    public ObservableProperty<int> Score { get; } = new();
    public ObservableProperty<string> HandType { get; } = new();
    public ObservableProperty<bool> CanPlay { get; } = new();
    
    public AsyncCommand PlayCardsCommand { get; }
    
    public GamePlayViewModel(IGameService gameService, IHandEvaluator handEvaluator)
    {
        _gameService = gameService;
        _handEvaluator = handEvaluator;
        PlayCardsCommand = new AsyncCommand(ExecutePlayCards, () => CanPlay.Value);
    }
    
    private async Task ExecutePlayCards()
    {
        var selectedCards = _gameService.GetSelectedCards();
        var handResult = _handEvaluator.EvaluateHand(selectedCards);
        var score = handResult.CalculateScore();
        
        await _gameService.PlayCards(selectedCards, handResult);
        
        // Automatically updates UI through binding
        Score.Value = score.Total;
        HandType.Value = handResult.HandType.ToString();
    }
}

// View - Only UI concerns
public class PlayCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI handTypeText;
    [SerializeField] private Button playButton;
    
    private GamePlayViewModel _viewModel;
    
    [Inject]
    public void Initialize(GamePlayViewModel viewModel)
    {
        _viewModel = viewModel;
        BindToViewModel();
    }
    
    private void BindToViewModel()
    {
        // Pure data binding - no business logic
        _viewModel.Score.Subscribe(score => scoreText.text = score.ToString());
        _viewModel.HandType.Subscribe(type => handTypeText.text = type);
        _viewModel.CanPlay.Subscribe(canPlay => playButton.interactable = canPlay);
        
        playButton.onClick.AddListener(() => _viewModel.PlayCardsCommand.Execute());
    }
}
```

### 3. Hard-coded Logic to Strategy Pattern

**Before: Monolithic Hand Evaluation**
```csharp
public class PlayCardView : MonoBehaviour
{
    private bool IsFlush()
    {
        if (_lstCardSelectingDeck.Count != 5) return false;
        var suit = _lstCardSelectingDeck[0].PokerCard.PokerSuit;
        foreach (var card in _lstCardSelectingDeck)
        {
            if (card.PokerCard.PokerSuit != suit) return false;
        }
        return true;
    }
    
    private bool IsStraight()
    {
        // 50+ lines of complex logic mixed with UI concerns
        _lstCardSelectingDeck.Sort((a, b) => a.PokerCard.PokerRank.CompareTo(b.PokerCard.PokerRank));
        // Complex straight detection with special cases...
    }
    
    private PokerHandValue EvaluateHand()
    {
        // Giant if-else chain
        if (IsRoyalFlush()) return PokerHandValue.RoyalFlush;
        if (IsStraightFlush()) return PokerHandValue.StraightFlush;
        if (IsFourOfAKind()) return PokerHandValue.FourOfAKind;
        // ... 20 more conditions
    }
}
```

**After: Clean Strategy Pattern**
```csharp
// Domain Service - Pure business logic
public class PokerHandEvaluator : IHandEvaluator
{
    private readonly Dictionary<PokerHandType, IHandPattern> _patterns;
    
    public PokerHandEvaluator()
    {
        _patterns = new Dictionary<PokerHandType, IHandPattern>
        {
            [PokerHandType.RoyalFlush] = new RoyalFlushPattern(),
            [PokerHandType.StraightFlush] = new StraightFlushPattern(),
            [PokerHandType.Flush] = new FlushPattern(),
            [PokerHandType.Straight] = new StraightPattern(),
            // ... other patterns
        };
    }
    
    public HandEvaluationResult EvaluateHand(IReadOnlyList<ICard> cards)
    {
        foreach (var pattern in _patterns.Values.OrderByDescending(p => p.Priority))
        {
            if (pattern.Matches(cards))
            {
                return new HandEvaluationResult(pattern.HandType, pattern.GetMatchingCards(cards));
            }
        }
        return HandEvaluationResult.HighCard(cards);
    }
}

// Individual pattern - Single responsibility
public class FlushPattern : IHandPattern
{
    public PokerHandType HandType => PokerHandType.Flush;
    public int Priority => 40;
    
    public bool Matches(IReadOnlyList<ICard> cards)
    {
        if (cards.Count < 5) return false;
        
        var suits = cards.Select(c => c.GetComponent<PlayableComponent>().Suit).Distinct();
        return suits.Count() == 1;
    }
    
    public IReadOnlyList<ICard> GetMatchingCards(IReadOnlyList<ICard> cards)
    {
        return cards.Take(5).ToList();
    }
}
```

## 🃏 Unified Card System Architecture

### Current Card System Problems
```csharp
// Separate inheritance hierarchies
public class PokerCard { /* poker-specific logic */ }
public class JokerCardVO { /* joker-specific logic */ }
public class TarotCard { /* tarot-specific logic */ }

// Enhancement logic scattered everywhere
public enum EnhancementsModifyCardEnum { Bonus, Mult, Wild, Glass, Steel, Stone, Gold, Lucky }
public enum EditionsModifyCardEnum { Base, Foil, Holographic, Polychrome, Negative }
public enum SealsModifyCardEnum { Gold, Red, Blue, Purple }

// Hard-coded switches for combinations
if (card.enhancement == Enhancement.Gold && card.seal == Seal.Red) {
    // Special case handling
}
```

### Unified Component-Based System

**Core Architecture**
```csharp
// Base interface for ALL cards
public interface ICard
{
    CardIdentity Identity { get; }
    string Name { get; }
    string Description { get; }
    Rarity Rarity { get; }
    
    // Component system - cards gain abilities through components
    T GetComponent<T>() where T : class, ICardComponent;
    void AddComponent(ICardComponent component);
    bool HasComponent<T>() where T : class, ICardComponent;
    
    // Modifier system - for enhancements, seals, editions
    void ApplyModifier(ICardModifier modifier);
    void RemoveModifier(ICardModifier modifier);
    IReadOnlyList<T> GetModifiers<T>() where T : ICardModifier;
    
    ICard Clone();
}

// Universal base implementation
public abstract class CardBase : ICard
{
    private readonly List<ICardComponent> _components = new();
    private readonly List<ICardModifier> _modifiers = new();
    
    protected CardBase(CardIdentity identity, string name, Rarity rarity)
    {
        Identity = identity;
        Name = name;
        Rarity = rarity;
    }
    
    public T GetComponent<T>() where T : class, ICardComponent
    {
        return _components.OfType<T>().FirstOrDefault();
    }
    
    public void ApplyModifier(ICardModifier modifier)
    {
        _modifiers.Add(modifier);
        modifier.OnApplied(this);
        RecalculateProperties(); // Auto-update when modifiers change
    }
    
    protected virtual void RecalculateProperties()
    {
        // Base properties get recalculated when modifiers change
        var newDescription = GetBaseDescription();
        foreach (var modifier in _modifiers)
        {
            newDescription = modifier.ModifyDescription(newDescription);
        }
        Description = newDescription;
    }
}
```

**Component System - Cards gain abilities**
```csharp
// Component for cards that can be played in poker hands
public class PlayableComponent : ICardComponent
{
    public Rank Rank { get; private set; }
    public Suit Suit { get; private set; }
    public int BaseChips { get; private set; }
    public int BaseMultiplier { get; private set; }
    
    // These get recalculated when modifiers applied
    public int EffectiveChips { get; private set; }
    public int EffectiveMultiplier { get; private set; }
    
    public void OnAttached(ICard card)
    {
        RecalculateEffectiveValues();
        card.OnCardEvent += OnCardModified;
    }
    
    private void OnCardModified(ICard card, CardEvent evt)
    {
        if (evt is CardModifiedEvent)
            RecalculateEffectiveValues();
    }
    
    private void RecalculateEffectiveValues()
    {
        EffectiveChips = BaseChips;
        EffectiveMultiplier = BaseMultiplier;
        
        // Apply all stat modifiers
        foreach (var modifier in _owner.GetModifiers<IStatModifier>())
        {
            EffectiveChips = modifier.ModifyChips(EffectiveChips);
            EffectiveMultiplier = modifier.ModifyMultiplier(EffectiveMultiplier);
        }
    }
}

// Component for cards with triggered effects (jokers, seals)
public class EffectComponent : ICardComponent
{
    private readonly List<ICardEffect> _effects = new();
    
    public void AddEffect(ICardEffect effect)
    {
        _effects.Add(effect);
    }
    
    public async Task<EffectResult> TriggerEffects(EffectContext context)
    {
        var results = new List<EffectResult>();
        
        foreach (var effect in _effects.Where(e => e.CanTrigger(context)))
        {
            var result = await effect.Execute(context);
            results.Add(result);
        }
        
        return EffectResult.Combine(results);
    }
}
```

**Modifier System - Composable enhancements**
```csharp
// Enhancement modifier (Bonus, Wild, Steel, etc.)
public class EnhancementModifier : ICardModifier, IStatModifier
{
    private readonly EnhancementType _type;
    
    public EnhancementModifier(EnhancementType type)
    {
        _type = type;
    }
    
    public int ModifyChips(int baseChips)
    {
        return _type switch
        {
            EnhancementType.Bonus => baseChips + 30,
            EnhancementType.Steel => baseChips * 2,
            EnhancementType.Stone => baseChips + 50,
            _ => baseChips
        };
    }
    
    public int ModifyMultiplier(int baseMultiplier)
    {
        return _type switch
        {
            EnhancementType.Mult => baseMultiplier + 4,
            EnhancementType.Wild => baseMultiplier + 0, // Wild affects suit/rank, not mult
            _ => baseMultiplier
        };
    }
    
    public string ModifyDescription(string baseDescription)
    {
        return _type switch
        {
            EnhancementType.Wild => $"{baseDescription} (Wild - counts as any suit/rank)",
            EnhancementType.Steel => $"{baseDescription} (Steel - 2x chips)",
            _ => $"{baseDescription} ({_type})"
        };
    }
}

// Seal modifier (Gold, Red, Blue, Purple)
public class SealModifier : ICardModifier
{
    private readonly SealType _sealType;
    
    public void OnApplied(ICard card)
    {
        // Add effect based on seal type
        var effectComponent = card.GetComponent<EffectComponent>() ?? new EffectComponent();
        
        ICardEffect sealEffect = _sealType switch
        {
            SealType.Gold => new GoldSealEffect(), // Creates $3 when played
            SealType.Red => new RedSealEffect(),   // Retriggers card
            SealType.Blue => new BlueSealEffect(), // Creates Planet card
            SealType.Purple => new PurpleSealEffect(), // Creates Tarot when discarded
            _ => null
        };
        
        if (sealEffect != null)
        {
            effectComponent.AddEffect(sealEffect);
            if (!card.HasComponent<EffectComponent>())
                card.AddComponent(effectComponent);
        }
    }
}
```

**Concrete Card Types**
```csharp
// Poker cards - playable with optional enhancements
public class PokerCard : CardBase
{
    public PokerCard(Rank rank, Suit suit) 
        : base(CardIdentity.Generate($"poker_{rank}_{suit}"), $"{rank} of {suit}", Rarity.Common)
    {
        // All poker cards can be played
        AddComponent(new PlayableComponent(rank, suit, GetBaseChips(rank), GetBaseMultiplier(rank)));
    }
    
    protected override string GetBaseDescription()
    {
        var playable = GetComponent<PlayableComponent>();
        return $"{playable.Rank} of {playable.Suit} - {playable.EffectiveChips} chips, {playable.EffectiveMultiplier} mult";
    }
}

// Joker cards - effect-based with optional modifiers
public class JokerCard : CardBase
{
    public JokerCard(JokerDefinition definition) 
        : base(CardIdentity.Generate($"joker_{definition.EffectName}"), definition.Name, definition.Rarity)
    {
        // All jokers have effects
        var effectComponent = new EffectComponent();
        AddComponent(effectComponent);
        
        // Create specific joker effect
        var jokerEffect = JokerEffectFactory.CreateEffect(definition);
        effectComponent.AddEffect(jokerEffect);
    }
}

// Tarot cards - consumable effects
public class TarotCard : CardBase
{
    private bool _isUsed;
    
    public TarotCard(TarotDefinition definition) 
        : base(CardIdentity.Generate($"tarot_{definition.Name}"), definition.Name, definition.Rarity)
    {
        var effectComponent = new EffectComponent();
        AddComponent(effectComponent);
        
        var tarotEffect = TarotEffectFactory.CreateEffect(definition);
        effectComponent.AddEffect(tarotEffect);
    }
    
    public async Task<bool> UseCard(EffectContext context)
    {
        if (_isUsed) return false;
        
        var effectComponent = GetComponent<EffectComponent>();
        var result = await effectComponent.TriggerEffects(context);
        
        if (result.IsSuccess)
        {
            _isUsed = true;
            return true;
        }
        return false;
    }
}
```

**Card Creation Examples**
```csharp
// Simple poker card
var aceOfSpades = new PokerCard(Rank.Ace, Suit.Spades);

// Enhanced poker card - any combination of modifiers!
var goldenWildAce = new PokerCard(Rank.Ace, Suit.Spades);
goldenWildAce.ApplyModifier(new EnhancementModifier(EnhancementType.Wild));
goldenWildAce.ApplyModifier(new EnhancementModifier(EnhancementType.Gold));
goldenWildAce.ApplyModifier(new SealModifier(SealType.Red));
goldenWildAce.ApplyModifier(new EditionModifier(EditionType.Holographic));
// Card automatically recalculates all properties!

// Fluent builder pattern for complex cards
var complexCard = CardBuilder.Create()
    .PokerCard(Rank.King, Suit.Hearts)
    .WithEnhancement(EnhancementType.Steel)
    .WithSeal(SealType.Blue)
    .WithEdition(EditionType.Foil)
    .Build();

// Joker cards
var abstractJoker = new JokerCard(JokerDefinitions.AbstractJoker);
```

## 🏭 Service Layer Implementation

### Current Manager to Service Transformation

**Before: Monolithic Managers**
```csharp
public class AudioManager : ManualSingletonMono<AudioManager>
{
    // Tons of Unity-specific code mixed with business logic
    public void PlaySFX(AudioName audioName) { /* complex implementation */ }
    public void PlayBGM(AudioName audioName) { /* complex implementation */ }
    // 500+ lines of audio logic
}
```

**After: Clean Service Interface**
```csharp
// Interface defines what we need, not how it's implemented
public interface IAudioService
{
    Task PlaySFX(AudioName audioName, float volume = 1.0f);
    Task PlayBGM(AudioName audioName, bool loop = true);
    void StopAll();
    void SetMasterVolume(float volume);
}

// Implementation hidden behind interface
public class UnityAudioService : IAudioService
{
    private readonly AudioSource _sfxSource;
    private readonly AudioSource _bgmSource;
    private readonly IAudioClipProvider _clipProvider;
    
    public UnityAudioService(IAudioClipProvider clipProvider)
    {
        _clipProvider = clipProvider;
        // Unity-specific setup
    }
    
    public async Task PlaySFX(AudioName audioName, float volume = 1.0f)
    {
        var clip = await _clipProvider.GetClip(audioName);
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
    }
}

// Easy to mock for testing
public class MockAudioService : IAudioService
{
    public List<AudioName> PlayedSounds { get; } = new();
    
    public Task PlaySFX(AudioName audioName, float volume = 1.0f)
    {
        PlayedSounds.Add(audioName);
        return Task.CompletedTask;
    }
}
```

### Game Service Implementation
```csharp
public interface IGameService
{
    Task<HandEvaluationResult> PlayCards(IReadOnlyList<ICard> cards);
    Task DiscardCards(IReadOnlyList<ICard> cards);
    Task EndTurn();
    IReadOnlyList<ICard> GetSelectedCards();
    GameState GetCurrentState();
}

public class GameService : IGameService
{
    private readonly IHandEvaluator _handEvaluator;
    private readonly IScoreService _scoreService;
    private readonly IPlayerService _playerService;
    private readonly IEventBus _eventBus;
    
    public GameService(
        IHandEvaluator handEvaluator, 
        IScoreService scoreService,
        IPlayerService playerService,
        IEventBus eventBus)
    {
        _handEvaluator = handEvaluator;
        _scoreService = scoreService;
        _playerService = playerService;
        _eventBus = eventBus;
    }
    
    public async Task<HandEvaluationResult> PlayCards(IReadOnlyList<ICard> cards)
    {
        // Pure business logic orchestration
        var handResult = _handEvaluator.EvaluateHand(cards);
        var score = await _scoreService.CalculateScore(handResult);
        
        await _playerService.AddScore(score);
        await _eventBus.PublishAsync(new CardsPlayedEvent(cards, handResult));
        
        return handResult;
    }
}
```

## 🔄 Step-by-Step Transformation Process

### Step 1: Extract Interfaces
1. Identify all singleton managers
2. Create interface for each manager
3. Extract interface with current public methods
4. No implementation changes yet

### Step 2: Implement Dependency Injection
1. Create DI container setup
2. Register services in container
3. Inject interfaces instead of accessing singletons
4. Keep old singletons working alongside new system

### Step 3: Extract Domain Logic
1. Identify pure business logic in managers
2. Create domain services and entities
3. Move business logic to domain layer
4. Managers become thin wrappers

### Step 4: Implement Unified Card System
1. Create ICard interface and CardBase
2. Migrate poker cards first (simplest)
3. Add component system
4. Add modifier system
5. Migrate jokers and other card types

### Step 5: Implement MVVM
1. Create ViewModel base classes
2. Extract UI logic from MonoBehaviour views
3. Implement data binding
4. Convert user actions to commands

### Step 6: Clean Up
1. Remove old singleton managers
2. Remove temporary compatibility code
3. Optimize and polish new system

This transformation creates a maintainable, testable, extensible architecture while preserving exact gameplay behavior.