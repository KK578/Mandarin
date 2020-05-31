namespace Mandarin.Models.Commissions
{
    public class SendRecordOfSalesModel
    {
        public SendRecordOfSalesModel(ArtistSales commission)
        {
            this.Commission = commission;
        }

        public ArtistSales Commission { get; }
        public string EmailAddress { get; set; }
        public string CustomMessage { get; set; }

        public override string ToString()
        {
            return $"{this.Commission} to {this.EmailAddress}";
        }
    }
}
