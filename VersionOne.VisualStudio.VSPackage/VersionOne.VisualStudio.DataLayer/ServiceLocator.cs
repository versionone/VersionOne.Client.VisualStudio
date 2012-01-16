using System;
using Ninject;

namespace VersionOne.VisualStudio.DataLayer {
    public class ServiceLocator {
        public IKernel Container { get; private set; }
        
        private ServiceLocator() { }

        private static ServiceLocator instance;

        public static ServiceLocator Instance {
            get { return instance ?? (instance = new ServiceLocator()); }
        }

        public void SetContainer(IKernel container) {
            if(container == null) {
                throw new ArgumentNullException("container");
            }

            Container = container;
        }

        public TService Get<TService>() {
            return Container.Get<TService>();
        }

        public TService Get<TService>(string name) {
            return Container.Get<TService>(name);
        }
    }
}