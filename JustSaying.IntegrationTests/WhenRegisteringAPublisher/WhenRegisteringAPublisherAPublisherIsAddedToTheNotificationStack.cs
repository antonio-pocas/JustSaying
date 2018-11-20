using System.Threading.Tasks;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Models;
using NSubstitute;
using Xunit;

namespace JustSaying.IntegrationTests.WhenRegisteringAPublisher
{
    [Collection(GlobalSetup.CollectionName)]
    public class WhenRegisteringAPublisher : FluentNotificationStackTestBase
    {
        private string _topicName;

        protected override async Task Given()
        {
            await base.Given();

            _topicName = "CustomerCommunication";

            EnableMockedBus();

            Configuration = new MessagingConfig();

            await DeleteTopicIfItAlreadyExists(_topicName);
        }

        protected override Task When()
        {
            SystemUnderTest.WithSnsMessagePublisher<Message>();
            return Task.CompletedTask;
        }

        [AwsFact]
        public void APublisherIsAddedToTheStack()
        {
            NotificationStack.Received().AddMessagePublisher<Message>(Arg.Any<IMessagePublisher>(), Region.SystemName);
        }

        [AwsFact]
        public void SerialisationIsRegisteredForMessage()
        {
            NotificationStack.SerialisationRegister.Received()
                .AddSerialiser<Message>(Arg.Any<IMessageSerialiser>());
        }

        protected override Task PostAssertTeardownAsync()
        {
            return DeleteTopicIfItAlreadyExists(_topicName);
        }
    }
}
