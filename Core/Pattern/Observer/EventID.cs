using System;

namespace Core.Controller.Observer
{
    [Serializable]
    public enum EventID
    {
        None = 0,
        OnShowMainMenuView,
        OnShowGamePlayView,
        OnShowResultGameView,
        OnShowShopView,
        OnShowOpenPackView,
        OnChangeStateGamePlay,
        OnActiveTutorial,
    }
    
}
