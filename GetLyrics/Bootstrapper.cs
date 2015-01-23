using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Core;
using System.Reflection;

namespace GetLyrics
{
    public class Bootstrapper : BootstrapperBase
    {
        private IWindsorContainer _windsorContainer;
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _windsorContainer = new WindsorContainer();

            _windsorContainer.Register(Component.For<IWindowManager>().ImplementedBy<MetroWindowManager>());
            _windsorContainer.Register(Component.For<IEventAggregator>().ImplementedBy<EventAggregator>());

            _windsorContainer.Register(
                Component.For<IShellViewModel>().ImplementedBy<ShellViewModel>().LifeStyle.Is(LifestyleType.Singleton));

            _windsorContainer.Register(
                Component.For<ICoverArtItemViewModel>()
                    .ImplementedBy<CoverArtItemViewModel>()
                    .LifeStyle.Is(LifestyleType.Singleton));

            _windsorContainer.Register(
                Component.For<IShellService>().ImplementedBy<ShellService>().LifeStyle.Is(LifestyleType.Singleton));
            _windsorContainer.Register(
                Component.For<IDialogService>().ImplementedBy<DialogService>().LifeStyle.Is(LifestyleType.Singleton));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key) ? _windsorContainer.Resolve(service) : _windsorContainer.Resolve<object>(key, new { });
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _windsorContainer.ResolveAll(service).Cast<object>();
        }

        protected override void BuildUp(object instance)
        {
            instance.GetType().GetProperties()
                .Where(property => property.CanWrite && property.PropertyType.IsPublic)
                .Where(property => _windsorContainer.Kernel.HasComponent(property.PropertyType)).ToList()
                .ForEach(property => property.SetValue(instance, _windsorContainer.Resolve(property.PropertyType), null));
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<IShellViewModel>();
        }
    }
}
