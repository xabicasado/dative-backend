using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using DativeBackend.Models;

namespace DativeBackend.Controllers {
    [Route("Api/[controller]")]
    [ApiController]
    [Authorize]  // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerController : ControllerBase {
        private readonly CustomerContext _context;
        private readonly IDistributedCache _distributedCache;
        
        public CustomerController(CustomerContext context, IDistributedCache distributedCache) {
            _context = context;
            _distributedCache = distributedCache;
        }

        // GET: Api/Customer/GetAllCustomers
        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomer() {
            List<CustomerDTO> customerList;
            // First attempt of retrieving from cache
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");
            /*
            customerList = await GetCacheCustomerList();
            if (!customerList.Any()) { }
            */
            if (encodedCustomerList != null) {
                string serializedCustomerList = Encoding.UTF8.GetString(encodedCustomerList);
                customerList = JsonConvert.DeserializeObject<List<CustomerDTO>>(serializedCustomerList);
            } else {
                // Retrieve the list from DB
                customerList = await _context.Customer
                    .Select(x => CustomerToDTO(x))
                    .ToListAsync();
                
                // await CacheStoreCustomerList(customerList);
                var cacheKey = "customerList";  // customerDTO.CustomerId.ToString();
                string serializedCustomer = JsonConvert.SerializeObject(customerList);
                var encodedCustomer = Encoding.UTF8.GetBytes(serializedCustomer);
                
                var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                                .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
                
                await _distributedCache.SetAsync(cacheKey, encodedCustomer, options);
            }           
            
            return customerList;
        }
        
        // GET: Api/Customer/GetCustomerData/5
        [HttpGet("GetCustomerData/{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id) {
            CustomerDTO customerDTO;
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");
            
            if (encodedCustomerList != null) {
                var cacheKey = id.ToString();
                string serializedCustomerList = Encoding.UTF8.GetString(encodedCustomerList);
                List<CustomerDTO> customerList = JsonConvert.DeserializeObject<List<CustomerDTO>>(serializedCustomerList);
                customerDTO = customerList.Find(customer => customer.CustomerId == id);

            } else {
                List<CustomerDTO> customerList = await _context.Customer
                    .Select(x => CustomerToDTO(x))
                    .ToListAsync();
                
                var cacheKey = "customerList";  // customerDTO.CustomerId.ToString();
                string serializedCustomer = JsonConvert.SerializeObject(customerList);
                var encodedCustomer = Encoding.UTF8.GetBytes(serializedCustomer);
                
                var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                                .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
                
                await _distributedCache.SetAsync(cacheKey, encodedCustomer, options);
                
                customerDTO = customerList.Find(customer => customer.CustomerId == id);
                /*
                Customer customer = await _context.Customer.FindAsync(id);
                if (customer == null) {
                    return NotFound();
                }
                
                await CacheStoreCustomer(CustomerToDTO(customer));
                customerDTO = CustomerToDTO(customer);
                */
            }
            if (customerDTO == null) {
                return NotFound();
            }

            return customerDTO;
        }

        // POST: api/Customer/CreateCustomer
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("CreateCustomer")]
        public async Task<ActionResult<CustomerDTO>> PostCustomer(Customer customer) {
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();
            
            CustomerDTO customerDTO = CustomerToDTO(customer);
            await CacheStoreCustomer(customerDTO);

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customerDTO);
        }

        private static CustomerDTO CustomerToDTO(Customer customer) =>
            new CustomerDTO {
                CustomerId = customer.CustomerId,
                PostalCode = customer.PostalCode,
                Name = customer.Name,
                Surname = customer.Surname,
                Age = customer.Age
            };
        
        // private async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCacheCustomerList() {
        private async Task<ActionResult<List<CustomerDTO>>> GetCacheCustomerList() {
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");
            
            return encodedCustomerList != null ? JsonConvert.DeserializeObject<List<CustomerDTO>>(Encoding.UTF8.GetString(encodedCustomerList)) : null;
        }
        
        private async Task CacheStoreCustomer(CustomerDTO customerDTO) {
            List<CustomerDTO> customerList;
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");
            
            if (encodedCustomerList != null) {
                string serializedCustomerList = Encoding.UTF8.GetString(encodedCustomerList);
                customerList = JsonConvert.DeserializeObject<List<CustomerDTO>>(serializedCustomerList);
                 
                customerList.Add(customerDTO);
            } else {
                // Retrieve the list from DB
                customerList = await _context.Customer
                    .Select(x => CustomerToDTO(x))
                    .ToListAsync();
                
                // await CacheStoreCustomerList(customerList);
            }

            var cacheKey = "customerList";  // customerDTO.CustomerId.ToString();
            string serializedCustomer = JsonConvert.SerializeObject(customerList);
            var encodedCustomer = Encoding.UTF8.GetBytes(serializedCustomer);
            
            var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
            
            await _distributedCache.SetAsync(cacheKey, encodedCustomer, options);
        }   
    }
}
