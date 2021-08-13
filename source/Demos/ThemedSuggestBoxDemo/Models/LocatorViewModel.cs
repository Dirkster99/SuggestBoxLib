namespace ThemedSuggestBoxDemo.Models
{
    public class LocatorViewModel : Infrastructure.ViewModelBase
    {
        /// <summary>
        /// Gets an instance of the service container and retrieves the requested service component.
        /// </summary>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        public TServiceContract GetService<TServiceContract>() where TServiceContract : class
        {
            return ServiceLocator.ServiceContainer.Instance.GetService<TServiceContract>();
        }
    }
}