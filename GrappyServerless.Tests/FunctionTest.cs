using System.IO;
using System.Linq;
using Amazon.Lambda.LexEvents;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Xunit;

namespace GrappyServerless.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void StartOrderingFlowersEventTest()
        {
            var json = File.ReadAllText("start-order-flowers-event.json");

            var lexEvent = JsonConvert.DeserializeObject<LexEvent>(json);

            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.FunctionHandler(lexEvent, context);

            Assert.Equal("Delegate", response.DialogAction.Type);
        }

        [Fact]
        public void CommitOrderingFlowersEventTest()
        {
            var json = File.ReadAllText("commit-order-flowers-event.json");

            var lexEvent = JsonConvert.DeserializeObject<LexEvent>(json);

            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.FunctionHandler(lexEvent, context);

            Assert.Equal("Close", response.DialogAction.Type);
            Assert.Equal("Fulfilled", response.DialogAction.FulfillmentState);
            Assert.Equal("PlainText", response.DialogAction.Message.ContentType);
            Assert.Contains("Thanks, your order for Roses has been placed", response.DialogAction.Message.Content);
        }

        [Fact]
        public void NegativePlayCardIntentTest()
        {
            var json = File.ReadAllText("show-play-card-event.json");
            var lexEvent = JsonConvert.DeserializeObject<LexEvent>(json);
            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.FunctionHandler(lexEvent, context);

            Assert.Equal("Close", response.DialogAction.Type);
            Assert.Equal("Fulfilled", response.DialogAction.FulfillmentState);
            Assert.Equal("SSML", response.DialogAction.Message.ContentType);
            Assert.Equal("<speak>Okay. Maybe some other time then.</speak>", response.DialogAction.Message.Content);
        }

        [Fact]
        public void PositivePlayCardIntentTest()
        {
            var json = File.ReadAllText("show-play-card-event.json");
            var lexEvent = JsonConvert.DeserializeObject<LexEvent>(json);

            if (lexEvent.CurrentIntent.Slots.ContainsKey("canStart"))
            {
                lexEvent.CurrentIntent.Slots["canStart"] = "yes";
            }

            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.FunctionHandler(lexEvent, context);

            Assert.Equal("EllicitSlot", response.DialogAction.Type);
            Assert.Null(response.DialogAction.FulfillmentState);
            Assert.Equal("SSML", response.DialogAction.Message.ContentType);
            Assert.Equal("<speak>Okay, here we go. Which animal do you see?</speak>", response.DialogAction.Message.Content);
            Assert.Equal("VirtualPlayCardIntent", response.DialogAction.IntentName);
            Assert.Equal("Animal", response.DialogAction.SlotToElicit);
            Assert.NotNull(response.DialogAction.ResponseCard);
            Assert.Equal("application/vnd.amazonaws.card.generic", response.DialogAction.ResponseCard.ContentType);
            Assert.NotNull(response.DialogAction.ResponseCard.GenericAttachments);
            Assert.NotEmpty(response.DialogAction.ResponseCard.GenericAttachments);
            Assert.NotNull(response.DialogAction.ResponseCard.GenericAttachments.Select(attachment => attachment.Buttons));
            Assert.NotEmpty(response.DialogAction.ResponseCard.GenericAttachments.Select(attachment => attachment.Buttons));
        }
    }
}
