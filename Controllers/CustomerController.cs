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
    [Authorize]
    public class CustomerController : ControllerBase {
        // Generation command (scaffolding): 
        // dotnet aspnet-codegenerator controller -name CustomerController -async -api -m Customer -dc CustomerContext -outDir Controllers
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
            
            if (encodedCustomerList != null) {
                customerList = GetDeserializedCustomerList(encodedCustomerList);
            } else {
                // Retrieve the list from DB and set cache 
                customerList = await GetCustomerList();
                await SetCacheStoreCustomerList(customerList);
            }           
            
            return customerList;
        }
        
        // GET: Api/Customer/GetCustomerData/5
        [HttpGet("GetCustomerData/{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id) {
            List<CustomerDTO> customerList;
            CustomerDTO customerDTO;
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");

            if (encodedCustomerList != null) {
                customerList = GetDeserializedCustomerList(encodedCustomerList);
            } else {
                customerList = await GetCustomerList();
                await SetCacheStoreCustomerList(customerList);
            }
            customerDTO = customerList.Find(customer => customer.CustomerId == id);

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
            await SetCacheStoreCustomer(customerDTO);

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customerDTO);
        }

        private async Task<List<CustomerDTO>> GetCustomerList() {
            return await _context.Customer
                .Select(x => CustomerToDTO(x))
                .ToListAsync();
        }

        private static CustomerDTO CustomerToDTO(Customer customer) =>
            new CustomerDTO {
                CustomerId = customer.CustomerId,
                PostalCode = customer.PostalCode,
                Name = customer.Name,
                Surname = customer.Surname,
                Age = customer.Age
            };
        
        private List<CustomerDTO> GetDeserializedCustomerList(byte[] encodedCustomerList) {
            return JsonConvert.DeserializeObject<List<CustomerDTO>>(Encoding.UTF8.GetString(encodedCustomerList));
        }
        
        private async Task SetCacheStoreCustomerList(List<CustomerDTO> customerList) {
            var cacheKey = "customerList";
            string serializedCustomer = JsonConvert.SerializeObject(customerList);
            var encodedCustomer = Encoding.UTF8.GetBytes(serializedCustomer);
            
            var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(1));
            
            await _distributedCache.SetAsync(cacheKey, encodedCustomer, options);
        }

        private async Task SetCacheStoreCustomer(CustomerDTO customerDTO) {
            List<CustomerDTO> customerList;
            var encodedCustomerList = await _distributedCache.GetAsync("customerList");
            
            if (encodedCustomerList != null) {
                customerList = GetDeserializedCustomerList(encodedCustomerList);
                customerList.Add(customerDTO);
            } else {
                customerList = await GetCustomerList();
            }

            await SetCacheStoreCustomerList(customerList);
        }
    }
}
