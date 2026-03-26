namespace FlyingShadow.Api.Repositories.Internal;

internal abstract class WithMockData<T>
{
    public abstract T LoadMockData();
}