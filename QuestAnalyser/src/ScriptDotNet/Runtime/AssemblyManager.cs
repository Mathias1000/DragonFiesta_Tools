﻿using ScriptNET.Runtime.Configuration;
using System;

namespace ScriptNET.Runtime
{
    public class AssemblyManager : BaseAssemblyManager, IAssemblyManager
    {
        #region Initialization

        [Bindable(false)]
        public override void Initialize(ScriptConfiguration configuration)
        {
            base.Initialize(configuration);

            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomainAssemblyLoad;
        }

        #endregion Initialization

        #region Overrides

        protected override void LoadAssemblies()
        {
            base.LoadAssemblies();

            WorkingAssemblies.Clear();
            WorkingAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        }

        private void CurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            AddAssembly(args.LoadedAssembly);
        }

        #endregion Overrides

        #region IDisposable Members

        [Bindable(false)]
        public override void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= CurrentDomainAssemblyLoad;
            base.Dispose();
        }

        #endregion IDisposable Members
    }
}