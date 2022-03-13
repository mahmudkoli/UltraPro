using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.MassTransit.Constants;
using UltraPro.MassTransit.Consumers;
using UltraPro.MassTransit.Messages;

namespace UltraPro.MassTransit.Configuration
{
    public static class MassTransitConfigurator
    {
        public static void ConfigurePublisher(this IServiceCollection services)
        {
            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(RabbitMqConsts.RabbitMqUri, "/", h =>
                    {
                        h.Username(RabbitMqConsts.UserName);
                        h.Password(RabbitMqConsts.Password);
                    });

                    cfg.Message<ISendReportRequest>(e => e.SetEntityName("report-requests")); // name of the primary exchange
                    cfg.Publish<ISendReportRequest>(e => e.ExchangeType = ExchangeType.Direct); // primary exchange type
                    cfg.Send<ISendReportRequest>(e =>
                    {
                        e.UseRoutingKeyFormatter(context => context.Message.Provider.ToString()); // route by provider (email or fax)
                    });
                });
            });
            services.AddMassTransitHostedService();
        }

        public static void ConfigureReceiver(this IServiceCollection services)
        {
            services.AddMassTransit(mt =>
            {
                mt.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(RabbitMqConsts.RabbitMqUri, "/", h =>
                    {
                        h.Username(RabbitMqConsts.UserName);
                        h.Password(RabbitMqConsts.Password);
                    });

                    cfg.ReceiveEndpoint("email-reports", re =>
                    {
                        // turns off default fanout settings
                        re.ConfigureConsumeTopology = false;

                        // a replicated queue to provide high availability and data safety. available in RMQ 3.8+
                        re.SetQuorumQueue();

                        // enables a lazy queue for more stable cluster with better predictive performance.
                        // Please note that you should disable lazy queues if you require really high performance, if the queues are always short, or if you have set a max-length policy.
                        re.SetQueueArgument("declare", "lazy");

                        //re.Consumer<EmailConsumer>();

                        re.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
                        re.UseMessageRetry(r =>
                        {
                            r.Immediate(5);
                            r.Handle<ArgumentNullException>();
                            r.Ignore(typeof(InvalidOperationException), typeof(InvalidCastException));
                            r.Ignore<ArgumentException>(t => t.ParamName == "orderTotal");
                        });
                        re.UseInMemoryOutbox();
                        re.Consumer(() => new EmailConsumer(), c => c.UseMessageRetry(r =>
                        {
                            r.Interval(10, TimeSpan.FromMilliseconds(200));
                            r.Ignore<ArgumentNullException>();
                        }));
                        re.DiscardFaultedMessages();

                        re.Bind("report-requests", e =>
                        {
                            e.RoutingKey = "email";
                            e.ExchangeType = ExchangeType.Direct;
                        });
                    });
                    cfg.ReceiveEndpoint("fax-reports", re =>
                    {
                        re.ConfigureConsumeTopology = false;
                        re.Consumer<FaxConsumer>();
                        re.Bind("report-requests", e =>
                        {
                            e.RoutingKey = "fax";
                            e.ExchangeType = ExchangeType.Direct;
                        });
                    });
                });
            });
            services.AddMassTransitHostedService();
        }
    }
}
