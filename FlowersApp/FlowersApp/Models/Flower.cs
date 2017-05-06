using System;

namespace FlowersApp.Models
{
    public class Flower
    {

        public int FlowerId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime? LastPurchase { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public string Observation { get; set; }
        public byte[] ImageArray { get; set; }
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(Image))
                { return "Flower.png"; }

                return string.Format("http://psflowersback.azurewebsites.net{0}", Image.Substring(1));
            }
        }
        public override int GetHashCode()
        {
            return FlowerId;
        }

    }
}
