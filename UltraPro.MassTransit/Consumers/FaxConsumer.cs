using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.MassTransit.Messages;

namespace UltraPro.MassTransit.Consumers
{
    public class FaxConsumer : IConsumer<ISendReportRequest>
    {
        public Task Consume(ConsumeContext<ISendReportRequest> context)
        {
            // do work to send fax here

            Console.Out.WriteLine("fax - " + context.Message.ReportId.ToString());

            Task.Delay(5000);

            return Task.CompletedTask;
        }
    }
}
