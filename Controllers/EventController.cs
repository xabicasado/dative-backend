using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Confluent.Kafka;
using DativeBackend.Models;

namespace DativeBackend.Controllers {
    [Route("Api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase {
        // TODO Set the connection params
        private ProducerConfig _producerConfig;

        public EventController(ProducerConfig producerConfig) {
            _producerConfig = producerConfig;
        }

        // POST: Api/Event/CustomerEvent
        [HttpPost("CustomerEvent")]
        [AllowAnonymous]
        public async Task<ActionResult> PostCustomerEvent(CustomerEvent customerEvent) {
            var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            await producer.ProduceAsync(customerEvent.EventType.ToString(), new Message<Null, string> { Value=customerEvent.Data });
            return null;
        }
    }
}
