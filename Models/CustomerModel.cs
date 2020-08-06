using System.Text.Json.Serialization;

namespace DativeBackend.Models {
    public class Customer {
        public int CustomerId { get; set; }
        public int PostalCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string Username { get; set; }
        // [JsonIgnore]
        public string Password { get; set; }
    }

    public class CustomerDTO {
        public int CustomerId { get; set; }
        public int PostalCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }

    public class CustomerLoginDTO {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}