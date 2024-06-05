using Raven.Client.Documents.Commands;

namespace Wolverine.Raven.Persistence;

public class StreamAsyncEnumerable<T> : IAsyncEnumerable<StreamResult<T>>
{
    private readonly IAsyncEnumerator<StreamResult<T>> stream;

    public StreamAsyncEnumerable(IAsyncEnumerator<StreamResult<T>> stream) => 
        this.stream = stream;

    public IAsyncEnumerator<StreamResult<T>> GetAsyncEnumerator(
        CancellationToken cancellationToken = new()) => stream;
}
