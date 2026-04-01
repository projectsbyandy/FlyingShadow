namespace FlyingShadow.Api.Repositories;

internal abstract class WithMockData<T>
{
    public abstract T LoadMockData();
}