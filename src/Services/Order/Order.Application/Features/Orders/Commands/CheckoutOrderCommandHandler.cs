using Grpc.Net.Client;
using MediatR;
using OrderGrpc.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrderClientGrpc.Protos.OrderProtoService;

namespace Order.Application.Features.Orders.Commands
{
    public class CheckoutOrderCommandHandler /*: IRequestHandler<CheckoutOrderCommand, int>*/
    {
        public CheckoutOrderCommandHandler()
        {
        }

        public void Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5045");
            var client = new OrderProtoServiceClient(channel);

        }
    }
}
