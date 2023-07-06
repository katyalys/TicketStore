using Identity.Application.Dtos.MailDto;
using Identity.Application.Interfaces;
using Identity.Infrastructure.Interfaces;
using MassTransit;
using Shared.EventBus.Messages.Events;
using System.Text;

namespace Identity.Application.MassTransit.Consumers
{
    public class EmailUpdatedInfoConsumer : IConsumer<UpdatedInfoEvent>
    {
        private readonly IMailService _mail;
        private readonly IUserAccessService _userService;

        public EmailUpdatedInfoConsumer(IMailService mail, IUserAccessService userService)
        {
            _mail = mail;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UpdatedInfoEvent> context)
        {
            var emailList = new List<string>();

            foreach (var userId in context.Message.UserIds)
            {
                var user = await _userService.GetByIdAsync(userId);
                emailList.Add(user.Email);
            }

            var mailData = new MailDataDto();
            mailData.ToMail = emailList;
            mailData.Subject = "Concert info has changed";
            mailData.Body = "The following changes have been made:\n";

            var messageBody = new StringBuilder();

            foreach (var kvp in context.Message.UpdatedProperties)
            {
                messageBody.Append($"{kvp.Key}: {kvp.Value} ");
            }

            mailData.Body += messageBody;

            await _mail.SendMailAsync(mailData, new CancellationToken());
        }
    }
}
