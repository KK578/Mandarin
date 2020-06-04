namespace Mandarin.Models.Commissions
{
    public class ArtistRecordOfSalesModel
    {
        public ArtistRecordOfSalesModel(ArtistSales commission)
        {
            this.Commission = commission;
        }

        public ArtistSales Commission { get; }
        public bool SendSuccessful { get; set; }
        public string StatusMessage { get; set; }
        public string EmailAddress { get; set; }
        public string CustomMessage { get; set; }

        public override string ToString()
        {
            return $"{this.Commission} to {this.EmailAddress}";
        }
    }
}
