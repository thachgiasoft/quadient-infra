using Autofac;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.Core.DependencyManagemenet
{
    /// <summary>
    /// AutoFac e register edilmesi gereken objelerin bulunduğu bir kütüphane
    /// var ise bu objelerin tamamını register eden bir sınıf yazılmalı 
    /// ve bu interface implement edilerek register işlemi gerçekleştirilmeli
    /// </summary>
    public interface INeedDependencyRegistrar
    {
        void Register(ContainerBuilder builder, ITypeFinder typeFinder);

        int Order { get; }
    }
}
