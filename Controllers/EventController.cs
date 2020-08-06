using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Confluent.Kafka;
using DativeBackend.Models;

namespace DativeBackend.Controllers {
    [Route("Api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase {
        private readonly ProducerConfig _producerConfig;

        public EventController(ProducerConfig producerConfig) {
            _producerConfig = producerConfig;
        }

        // POST: Api/Event/CustomerEvent
        [HttpPost("CustomerEvent")]
        [AllowAnonymous]
        public async Task<ActionResult> PostCustomerEvent(CustomerEvent customerEvent) {
            return null;
        }
    }
}
