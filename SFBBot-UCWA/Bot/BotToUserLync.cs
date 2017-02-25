using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFBBot_UCWA.UcwaSfbo
{
    public class BotToUserLync : IBotToUser
    {
        private IMessageActivity toBot;

        public BotToUserLync(IMessageActivity toBot)
        {
            SetField.NotNull(out this.toBot, nameof(toBot), toBot);
        }

        public IMessageActivity MakeMessage() => toBot;

        public async Task PostAsync(IMessageActivity message, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sendMessageUrl = message.ChannelData as string;
            await UcwaSendMessage.SendIM_Step05(Program.httpClient, message.Text, sendMessageUrl);
        }
    }
}
