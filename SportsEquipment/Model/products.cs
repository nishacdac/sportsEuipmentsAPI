using System.ComponentModel.DataAnnotations;

namespace SportsEquipment.Model
{
    public class products
    {
        internal object in_Active;
        public int product_ID { get; set; }
        public string product_name { get; set; }
        public string material_type { get; set; }
        public string image_url { get; set; }
        public bool in_active { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public DateTime create_At { get; set; }
        public DateTime update_At { get; set; }
        public int category_id { get; set; }
    }


    //////////////Admin chart show for products/////////////

    public class ProductChart
    {
        public string product_name { get; set; }
        public int total_quantity { get; set; }

    }

   

}






