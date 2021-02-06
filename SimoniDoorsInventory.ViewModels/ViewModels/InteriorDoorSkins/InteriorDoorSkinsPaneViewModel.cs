using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using SimoniDoorsInventory.Data;
using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class InteriorDoorSkinsPaneViewModel : ViewModelBase
    {
        public InteriorDoorSkinsPaneViewModel(IInteriorDoorSkinService interiorDoorSkinService,
                                              ICommonServices commonServices) : base(commonServices)
        {
            InteriorDoorSkinService = interiorDoorSkinService;
        }

        public IInteriorDoorSkinService InteriorDoorSkinService { get; }

        private IList<InteriorDoorSkinModel> _interiorDoorSkins = null;
        public IList<InteriorDoorSkinModel> InteriorDoorSkins
        {
            get => _interiorDoorSkins;
            set => Set(ref _interiorDoorSkins, value);
        }

        public async Task LoadAsync()
        {
            StartStatusMessage("Φόρτωση επενδύσεων μεσόπορτας...");
            await LoadInteriorDoorSkinsAsync();
            EndStatusMessage("Επενδύσεις μεσόπορτας φορτώθηκαν");
        }
        public void Unload()
        {
            InteriorDoorSkins = null;
        }

        private async Task LoadInteriorDoorSkinsAsync()
        {
            try
            {
                var request = new DataRequest<InteriorDoorSkin>
                {
                    OrderBy = r => r.InteriorDoorSkinID
                };
                // InteriorDoorSkins = await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(0, 10, request);
                InteriorDoorSkins = await InteriorDoorSkinService.GetInteriorDoorSkinsAsync(0, 10, request);
            }
            catch (Exception ex)
            {
                LogException("Dashoard", "Load Interior Door Skins", ex);
            }
        }
    }
}
