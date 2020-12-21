namespace SimoniDoorsInventory
{
    public class UserInfo
    {
        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public object PictureSource { get; set; }

        public string DisplayName => $"{FirstName} {LastName}";

        public bool IsEmpty => string.IsNullOrEmpty(DisplayName.Trim());

        static readonly public UserInfo Default = new UserInfo
        {
            AccountName = "A.B. Cuka",
            FirstName = "A.B.",
            LastName = "Cuka"
        };
    }
}
