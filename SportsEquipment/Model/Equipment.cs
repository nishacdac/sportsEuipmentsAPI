namespace SportsEquipment.Model 
{
    public class Equipment
    {
        //internal object address;
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }


        public string? Address { get; internal set; }
        public string? Image { get; internal set; }

    }

    public class AdminLogin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }

        public string FirstName { get; set; }

    }


}

