using System.Collections.Generic;
using System;
using System.Reflection;
using Framework.Core;

namespace Framework.MVVM {

    public class PropertyBinder<ViewModelBase> {

        private delegate void BindHandler(ViewModelBase viewModel);
        private delegate void UnBindHandler(ViewModelBase viewModel);

        private readonly List<BindHandler> binders = new List<BindHandler>();
        private readonly List<UnBindHandler> unBinders = new List<UnBindHandler>();

        public void Add<TProperty>(string name, string realTypeName,
                                   Action<TProperty, TProperty> valueChangedHandler) {
            var fieldInfo = GameApplication.Instance.HotFix.LoadType(realTypeName)
                .GetField(name, BindingFlags.Instance | BindingFlags.Public);
            
            if (fieldInfo == null) 
                throw new Exception(string.Format("Unable to find bindableproperty field '{0}.{1}'", realTypeName, name));
            
            binders.Add(viewModel => {
                GetPropertyValue<TProperty>(name, viewModel, realTypeName, fieldInfo).OnValueChanged += valueChangedHandler;
            });
            unBinders.Add(viewModel => {
                GetPropertyValue<TProperty>(name, viewModel, realTypeName, fieldInfo).OnValueChanged -= valueChangedHandler;
            });
        }
        
        private BindableProperty<TProperty> GetPropertyValue<TProperty>(string name, ViewModelBase viewModel, string realTypeName, FieldInfo fieldInfo) {
            var value = fieldInfo.GetValue(viewModel);
            BindableProperty<TProperty> bindableProperty = value as BindableProperty<TProperty>;
            if (bindableProperty == null) 
                throw new Exception(string.Format("Illegal bindableproperty field '{0}.{1}' ", realTypeName, name));
            return bindableProperty;
        }
        public void Bind(ViewModelBase viewModel) {
            if (viewModel != null) 
                for (int i = 0; i < binders.Count; i++) 
                    binders[i](viewModel);
        }
        public void UnBind(ViewModelBase viewModel) {
            if (viewModel != null) 
                for (int i = 0; i < unBinders.Count; i++) 
                    unBinders[i](viewModel);
        }
    }
}
