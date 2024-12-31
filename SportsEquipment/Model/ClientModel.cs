namespace SportsEquipment.Models
{
    public class ClientModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Gender { get; set; }
    }

    public class Cart
    {
        public int id { get; set; }
        public int customer_id { get; set; }
        public int product_id { get; set; }
        public DateTime date { get; set; }
        public int quantity { get; set; }
        public int TotalProducts { get; set; } // Add this property
    }

    // get krte time ka sara dta yeh hai
    public class CartItem
    {
        public DateTime date { get; set; }
        public int quantity { get; set; }
        public int customer_id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string product_name { get; set; }
        public int product_id { get; set; }

        public string image_url { get; set; }
        public decimal price { get; set; }
    }




    public class DateSummary
    {
        //public DateTime DateOnly { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalPrice { get; set; }
    }


    // seler table for get product nam eor product quantity for chart graph table

    public class SellerData
    {
        //public int product_name { get; set; }
        //public int TotalQuantity { get; set; }
        public string first_name { get; set; }
        public string product_name { get; set; }  // Ensure this is a string
        public int quantity { get; set; }
    }



    //nisha ka code hai yeh edit wala

    public class EditModel
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Image { get; set; }
        public string? Gender { get; set; }
        public string? MobileNumber { get; set; }
    }


    /// <summary>
    /// ///////////Reset-Password
    /// </summary>

    public class ResetPasswordModel
    {

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }

    /// <summary>
    /// ///////////seller-profile for seller
    /// </summary>
    public class SellerProfileModel
    {

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string Image { get; set; }
        public string? Gender { get; set; }
        public string? MobileNumber { get; set; }
    }



    //////////////////Seller-Password///////////////////

    public class SellerPasswordModel
    {

        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }



}