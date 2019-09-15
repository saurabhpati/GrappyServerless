using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using static Amazon.Lambda.LexEvents.LexResponse;

namespace GrappyServerless
{
    public class ResolvePlayCardIntentProcessor : AbstractIntentProcessor
    {
        private const string CAN_START = "canStart";
        private const string ANIMAL = "Animal";

        public override LexResponse Process(LexEvent lexEvent, ILambdaContext context)
        {
            var slots = lexEvent.CurrentIntent.Slots;

            var sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();

            if (slots.All(s => string.IsNullOrWhiteSpace(s.Value)))
            {
                return Delegate(sessionAttributes, slots);
            }

            if (!slots[CAN_START].ToProcessorBool())
            {
                return Close(
                    sessionAttributes,
                    "Fulfilled",
                    new LexMessage()
                    {
                        ContentType = "SSML",
                        Content = "<speak>Okay. Maybe some other time then.</speak>"
                    });
            }

            if (!slots.ContainsKey(ANIMAL))
            {
                slots.Add(ANIMAL, string.Empty);
            }

            return ElicitSlot(
                sessionAttributes,
                new LexDialogAction()
                {
                    IntentName = "VirtualPlayCardIntent",
                    Slots = slots,
                    SlotToElicit = ANIMAL,
                    Message = new LexMessage()
                    {
                        ContentType = "PlainText",
                        Content = "Okay, here we go. Which animal do you see?"
                    },
                    ResponseCard = new LexResponseCard()
                    {
                        Version = 1,
                        ContentType = "application/vnd.amazonaws.card.generic",
                        GenericAttachments = new List<LexGenericAttachments>()
                        {
                            new LexGenericAttachments()
                            {
                                Title = "What do you see?",
                                SubTitle = "What do yo see in the picture?",
                                ImageUrl = "https://placeimg.com/220/220/animals",
                                Buttons = new List<LexButton>()
                                {
                                    new LexButton()
                                    {
                                        Text = "Dog",
                                        Value = "Dog"
                                    },
                                    new LexButton()
                                    {
                                        Text = "Cat",
                                        Value = "Dog"
                                    },
                                    new LexButton()
                                    {
                                        Text = "Bird",
                                        Value = "Dog"
                                    },
                                    new LexButton()
                                    {
                                        Text = "Other",
                                        Value = "Other"
                                    },
                                }
                            }
                        }
                    }
                });
        }
    }
}
