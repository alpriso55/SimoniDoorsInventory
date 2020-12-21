using System;
using System.Threading.Tasks;

using SimoniDoorsInventory.Models;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.ViewModels
{
    public class PaymentsViewModel : ViewModelBase
    {
        public PaymentsViewModel(IPaymentService paymentService, 
                                 ICommonServices commonServices) : base(commonServices)
        {
            PaymentService = paymentService;

            PaymentList = new PaymentListViewModel(PaymentService, commonServices);
            PaymentDetails = new PaymentDetailsViewModel(PaymentService, commonServices);
        }

        public IPaymentService PaymentService { get; }

        public PaymentListViewModel PaymentList { get; set; }
        public PaymentDetailsViewModel PaymentDetails { get; set; }

        public async Task LoadAsync(PaymentListArgs args)
        {
            await PaymentList.LoadAsync(args);
        }
        public void Unload()
        {
            PaymentDetails.CancelEdit();
            PaymentList.Unload();
        }

        public void Subscribe()
        {
            MessageService.Subscribe<PaymentListViewModel>(this, OnMessage);
            PaymentList.Subscribe();
            PaymentDetails.Subscribe();
        }
        public void Unsubscribe()
        {
            MessageService.Unsubscribe(this);
            PaymentList.Unsubscribe();
            PaymentDetails.Unsubscribe();
        }

        private async void OnMessage(PaymentListViewModel viewModel, string message, object args)
        {
            if (viewModel == PaymentList && message == "ItemSelected")
            {
                await ContextService.RunAsync(() =>
                {
                    OnItemSelected();
                });
            }
        }

        private async void OnItemSelected()
        {
            if (PaymentDetails.IsEditMode)
            {
                StatusReady();
                PaymentDetails.CancelEdit();
            }
            var selected = PaymentList.SelectedItem;
            if (!PaymentList.IsMultipleSelection)
            {
                if (selected != null && !selected.IsEmpty)
                {
                    await PopulateDetails(selected);
                }
            }
            PaymentDetails.Item = selected;
        }

        private async Task PopulateDetails(PaymentModel selected)
        {
            try
            {
                var model = await PaymentService.GetPaymentAsync(selected.PaymentID);
                selected.Merge(model);
            }
            catch (Exception ex)
            {
                LogException("Payments", "Load Details", ex);
            }
        }
    }
}
