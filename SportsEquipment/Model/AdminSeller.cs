namespace SportsEquipment.Model
{
    public class AdminSeller
    {
        public int seller_Id { get; set; }
        public string company_name { get; set; }
        public string gst_number { get; set; }
        public string company_address { get; set; }
        public string company_email { get; set; }
        public string company_mobile_number { get; set; } 
        public string logo { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string mobile_number { get; set; }
        public string address { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string product_id { get; set; }
    }


    public class AdminProfile
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfileImageUrl { get; set; }  // URL to the profile image
    }


    public class AdminOrder
    {
        public int customer_id { get; set; }
        public string MobileNumber { get; set; }
        public string FirstName { get; set; }
        public string product_name { get; set; }

        
        public decimal quantity { get; set; }
        public string PaymentStatus { get; set; }
    }

}
