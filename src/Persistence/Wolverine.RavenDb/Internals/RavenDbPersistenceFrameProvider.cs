using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;
using Raven.Client.Documents.Session;
using Wolverine.Configuration;
using Wolverine.Persistence;
using Wolverine.Persistence.Sagas;
using Wolverine.Runtime;
using MethodCall = JasperFx.CodeGeneration.Frames.MethodCall;


namespace Wolverine.RavenDb.Internals;

// ReSharper disable once InconsistentNaming
internal class RavenDbPersistenceFrameProvider : IPersistenceFrameProvider
{
    public bool CanPersist(Type entityType, IContainer container, out Type persistenceService)
    {
        persistenceService = typeof(IAsyncDocumentSession);
        return true;
    }

    public Type DetermineSagaIdType(Type sagaType, IContainer container)
    {
        //Raven only supports string ids
        return typeof(string);
    }

    public Frame DetermineLoadFrame(IContainer container, Type sagaType, Variable sagaId)
    {
        return new LoadDocumentFrame(sagaType, sagaId);
    }

    public Frame DetermineInsertFrame(Variable saga, IContainer container)
    {
        return new DocumentSessionOperationFrame(saga, nameof(IAsyncDocumentSession.StoreAsync));
    }

    public Frame CommitUnitOfWorkFrame(Variable saga, IContainer container)
    {
        var call = MethodCall.For<IAsyncDocumentSession>(x => x.SaveChangesAsync(default));
        call.CommentText = "Commit all pending changes";
        return call;
    }

    public Frame DetermineUpdateFrame(Variable saga, IContainer container)
    {
        return new CommentFrame("No explicit update necessary with Raven");
    }

    public Frame DetermineDeleteFrame(Variable sagaId, Variable saga, IContainer container)
    {
        return new DocumentSessionOperationFrame(saga, nameof(IAsyncDocumentSession.Delete));
    }

    public void ApplyTransactionSupport(IChain chain, IContainer container)
    {
        if (!chain.Middleware.OfType<TransactionalFrame>().Any())
        {
            chain.Middleware.Add(new TransactionalFrame(chain));

            if (chain is not SagaChain)
            {
                var saveChanges = MethodCall.For<IAsyncDocumentSession>(x => x.SaveChangesAsync(default));
                saveChanges.CommentText = "Commit any outstanding Raven changes";
                chain.Postprocessors.Add(saveChanges);

                var methodCall = MethodCall.For<MessageContext>(x => x.FlushOutgoingMessagesAsync());
                methodCall.CommentText = "Flushing messages after save";

                chain.Postprocessors.Add(methodCall);
            }
        }
    }

    public bool CanApply(IChain chain, IContainer container)
    {
        return chain is SagaChain 
               || chain.ServiceDependencies(container, new[] { typeof(IAsyncDocumentSession) }).Any(x => x == typeof(IAsyncDocumentSession));
    }

}
