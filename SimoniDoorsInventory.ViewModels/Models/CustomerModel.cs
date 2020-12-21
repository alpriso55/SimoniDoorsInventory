using System;
using SimoniDoorsInventory.Services;

namespace SimoniDoorsInventory.Models
{
    public class CustomerModel : ObservableObject
    {
        static public CustomerModel CreateEmpty() => new CustomerModel
        {
            CustomerID = -1,
            IsEmpty = true
        };

        public long CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int Floor { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
        public string Observations { get; set; }

        public byte[] Picture { get; set; }
        public object PictureSource { get; set; }

        public byte[] Thumbnail { get; set; }
        public object ThumbnailSource { get; set; }

        // --------------------------------------------------------
        public bool IsNew => CustomerID <= 0;
        public string FullName => $"{FirstName} {LastName}";
        public string Initials => string.Format("{0}{1}", $"{FirstName} "[0], $"{LastName} "[0]).Trim().ToUpper();

        public override string ToString()
        {
            return IsEmpty ? "Ανώνυμος" : FullName;
        }

        public string FullAddress
        {
            get
            {
                return $"{AddressLine}, {City} {PostalCode}, {Floor}ος";
            }
        }

        public override void Merge(ObservableObject source)
        {
            if (source is CustomerModel model)
            {
                Merge(model);
            }
        }
        public void Merge(CustomerModel source)
        {
            if (source != null)
            {
                CustomerID = source.CustomerID;
                FirstName = source.FirstName;
                LastName = source.LastName;
                Phone1 = source.Phone1;
                Phone2 = source.Phone2;
                Email = source.Email;
                AddressLine = source.AddressLine;
                City = source.City;
                PostalCode = source.PostalCode;
                Floor = source.Floor;
                CreatedOn = source.CreatedOn;
                LastModifiedOn = source.LastModifiedOn;
                Observations = source.Observations;
                Thumbnail = source.Thumbnail;
                ThumbnailSource = source.ThumbnailSource;
                Picture = source.Picture;
                PictureSource = source.PictureSource;
            }
        }
    }
}
