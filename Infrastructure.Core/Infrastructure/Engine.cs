using System.Configuration;
using Autofac;
using Infrastructure.Core.Configuration;
using Infrastructure.Core.DependencyManagemenet;
using System;
using Infrastructure.Core.TypeFinders;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Settings;
using System.Diagnostics;
using Infrastructure.Core.Logging;
using System.Linq;

namespace Infrastructure.Core.Infrastructure
{
    internal class Engine : IEngine
    {
        #region Fields

        private ContainerManager _containerManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public Engine()
            : this(EventBroker.Instance)
        {
        }

        public Engine(EventBroker broker)
        {
            var config = ConfigurationManager.GetSection("coreConfiguration") as CoreConfiguration;
            if (config == null)
                throw new ConfigurationErrorsException(string.Format(
                    "coreConfiguration section not found!. Example {0}", CoreConsts.CoreConfigurationSample));
            InitializeContainer(broker, config);
        }

        #endregion

        #region Utilities

        // ReSharper disable UnusedMember.Local
        private void RunStartupTasks()
        // ReSharper restore UnusedMember.Local
        {
#pragma warning disable 168
            var typeFinder = _containerManager.Resolve<ITypeFinder>();
#pragma warning restore 168
            //var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            //var startUpTasks = new List<IStartupTask>();
            //foreach (var startUpTaskType in startUpTaskTypes)
            //    startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            //startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            //foreach (var startUpTask in startUpTasks)
            //    startUpTask.Execute();
        }

        private void InitializeContainer(EventBroker broker, CoreConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            //other dependencies
            builder.RegisterInstance(configuration).Keyed("core.configuration", typeof(CoreConfiguration)).As<CoreConfiguration>().SingleInstance();

            builder.RegisterInstance(this).Keyed<IEngine>("core.engine").As<IEngine>().SingleInstance();
            //containerManager.AddComponentInstance<FirstContainerConfigurer>(this, "core.containerConfigurer");
            builder.RegisterType<ConfigurationManagerWrapper>().Keyed<IConfigurationManager>("core.configurationManagerWrapper").As<IConfigurationManager>().SingleInstance();
            builder.RegisterType<CoreCryptography>().Keyed<ICoreCryptography>("core.cryptography").As<ICoreCryptography>().SingleInstance();
            var notificationInstance = new SettingNotification();
            builder.RegisterInstance(notificationInstance).SingleInstance();
            builder.RegisterInstance(new EventLog() { Source = "Application" }).SingleInstance();
            builder.RegisterType<DefaultLogger>().Keyed<ILogger>(CoreConsts.DefaultLoggerKey).SingleInstance();
            //type finder
            var typeFinderInstance = new WebAppTypeFinder(configuration);
            builder.RegisterInstance(typeFinderInstance).Keyed<ITypeFinder>("core.typeFinder").As<ITypeFinder>().SingleInstance();


            //register dependencies provided by other assemblies

            var drTypes = typeFinderInstance.FindClassesOfType<INeedDependencyRegistrar>();
            var drInstances = drTypes.Select(drType => (INeedDependencyRegistrar)Activator.CreateInstance(drType)).ToList();
            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinderInstance);

            //event broker
            builder.RegisterInstance(broker).SingleInstance();

            //Setting Subscribe

            if (notificationInstance != null)
            {
                foreach (var settingObserver in typeFinderInstance.FindClassesOfType<BaseSettingObserver>())
                {
                    var type = Activator.CreateInstance(settingObserver) as BaseSettingObserver;
                    if (type != null)
                        notificationInstance.Subscribe(type);
                }
            }

            _containerManager = new ContainerManager(builder.Build(), configuration);

        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the core environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize(CoreConfiguration config)
        {
            //bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();
            //if (databaseInstalled)
            //{
            //    //startup tasks
            //    RunStartupTasks();
            //}
        }

        public T Resolve<T>(string key = "") where T : class
        {
            return ContainerManager.Resolve<T>(key);
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public bool TryResolve(Type serviceType, out object instance)
        {
            return ContainerManager.TryResolve(serviceType, out instance);
        }

        public bool IsRegistered<T>(string key = "")
        {
            return ContainerManager.IsRegistered<T>(key);
        }

        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        public ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
    }
}