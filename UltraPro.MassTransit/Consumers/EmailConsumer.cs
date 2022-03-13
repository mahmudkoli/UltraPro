using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.MassTransit.Messages;

namespace UltraPro.MassTransit.Consumers
{
    public class EmailConsumer : IConsumer<ISendReportRequest>
    {
        public Task Consume(ConsumeContext<ISendReportRequest> context)
        {
            try
            {
                // do work to send email here

                Console.Out.WriteLine("email - " + context.Message.ReportId.ToString());

                Task.Delay(5000);
            }
            catch (Exception ex)
            {
                // schedule redelivery in one minute
                context.Redeliver(TimeSpan.FromMinutes(1));

                // schedule redelivery in one minute
                context.Defer(TimeSpan.FromMinutes(1));
            }

            return Task.CompletedTask;
        }
    }
}
