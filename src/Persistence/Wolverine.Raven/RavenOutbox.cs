﻿using Raven.Client.Documents.Session;
using Wolverine.Runtime;

namespace Wolverine.Raven;

public class RavenOutbox : MessageContext, IRavenOutbox
{
    public RavenOutbox(IWolverineRuntime runtime, IAsyncDocumentSession session) : base(runtime)
    {
        Session = session;
    }

    public IAsyncDocumentSession Session { get; }
    
    public async Task SaveChangesAndFlushMessagesAsync(CancellationToken token = default)
    {
        await Session.SaveChangesAsync(token);
        
        await FlushOutgoingMessagesAsync();
    }
}
