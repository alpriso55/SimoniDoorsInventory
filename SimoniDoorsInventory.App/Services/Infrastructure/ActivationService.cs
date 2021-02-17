#region copyright
// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************
#endregion

using System;

using Windows.ApplicationModel.Activation;

using SimoniDoorsInventory.ViewModels;

namespace SimoniDoorsInventory.Services
{
    #region ActivationInfo
    public class ActivationInfo
    {
        static public ActivationInfo CreateDefault() => Create<DashboardViewModel>();

        static public ActivationInfo Create<TViewModel>(object entryArgs = null) where TViewModel : ViewModelBase
        {
            return new ActivationInfo
            {
                EntryViewModel = typeof(TViewModel),
                EntryArgs = entryArgs
            };
        }

        public Type EntryViewModel { get; set; }
        public object EntryArgs { get; set; }
    }
    #endregion

    static public class ActivationService
    {
        static public ActivationInfo GetActivationInfo(IActivatedEventArgs args)
        {
            switch (args.Kind)
            {
                case ActivationKind.Protocol:
                    return GetProtocolActivationInfo(args as ProtocolActivatedEventArgs);

                case ActivationKind.Launch:
                default:
                    return ActivationInfo.CreateDefault();
            }
        }

        private static ActivationInfo GetProtocolActivationInfo(ProtocolActivatedEventArgs args)
        {
            if (args != null)
            {
                switch (args.Uri.AbsolutePath.ToLowerInvariant())
                {
                    case "customer":
                    case "customers":
                        long customerID = args.Uri.GetInt64Parameter("id");
                        if (customerID > 0)
                        {
                            return ActivationInfo.Create<CustomerDetailsViewModel>(new CustomerDetailsArgs { CustomerID = customerID });
                        }
                        return ActivationInfo.Create<CustomersViewModel>(new CustomerListArgs());
                    case "order":
                    case "orders":
                        long orderID = args.Uri.GetInt64Parameter("id");
                        if (orderID > 0)
                        {
                            return ActivationInfo.Create<OrderDetailsViewModel>(new OrderDetailsArgs { OrderID = orderID });
                        }
                        return ActivationInfo.Create<OrdersViewModel>(new OrderListArgs());
                    case "payment":
                    case "payments":
                        long paymentID = args.Uri.GetInt64Parameter("id");
                        if (paymentID > 0)
                        {
                            return ActivationInfo.Create<PaymentDetailsViewModel>(new PaymentDetailsArgs { PaymentID = paymentID });
                        }
                        return ActivationInfo.Create<PaymentsViewModel>(new PaymentListArgs());
                    case "interiordoorskin":
                    case "interiordoorskins":
                        string interiorDoorSkinID = args.Uri.GetParameter("id");
                        if (!string.IsNullOrWhiteSpace(interiorDoorSkinID))
                        {
                            return ActivationInfo.Create<InteriorDoorSkinDetailsViewModel>(new InteriorDoorSkinDetailsArgs { InteriorDoorSkinID = interiorDoorSkinID });
                        }
                        return ActivationInfo.Create<InteriorDoorSkinsViewModel>(new InteriorDoorSkinListArgs());
                }
            }
            return ActivationInfo.CreateDefault();
        }
    }
}
