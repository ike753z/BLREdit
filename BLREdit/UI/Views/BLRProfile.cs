﻿using BLREdit.Export;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BLREdit.UI.Views;

public sealed class BLRProfile : INotifyPropertyChanged
{
    #region Event
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); }
    #endregion Event

    private BLRLoadout loadout1 = new();
    public BLRLoadout Loadout1 { get { return loadout1; } }

    private BLRLoadout loadout2 = new();
    public BLRLoadout Loadout2 { get { return loadout2; } }
    
    private BLRLoadout loadout3 = new();
    public BLRLoadout Loadout3 { get { return loadout3; } }

    public void UpdateSearchAndFilter()
    {
        MainWindow.Self.ApplySearchAndFilter();
    }

    private MagiCowsProfile internalProfile;
    public void LoadMagicCowsProfile(MagiCowsProfile profile)
    {
        internalProfile = profile;
        loadout1.LoadMagicCowsLoadout(internalProfile.Loadout1);
        loadout2.LoadMagicCowsLoadout(internalProfile.Loadout2);
        loadout3.LoadMagicCowsLoadout(internalProfile.Loadout3);
    }

    public void UpdateMagicCowsProfile()
    {
        loadout1.UpdateMagicCowsLoadout();
        loadout2.UpdateMagicCowsLoadout();
        loadout3.UpdateMagicCowsLoadout();
    }
}
