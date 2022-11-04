using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core {

// 为System.Collections.Generic.IList接口提供必要的适配: 因为保存加载系统现在放在主工程中,这个类不需要了.但是本身这个类并没有连通
    public class IListCrossBindingAdapter : CrossBindingAdaptor {

// 实现基类的三个方法        
        public override Type BaseCLRType {
            get {
                return typeof(IList);
            }
        }
        public override Type AdaptorType {
            get {
                return typeof(IEnumeratorObjectAdaptor.Adaptor);
            }
        }
        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain,
                                                 ILTypeInstance instance) {
            return new IListCrossBindingAdapter.IListAdaptor(appdomain, instance);
        }

		public class IListAdaptor : IList<ILTypeInstance>, CrossBindingAdaptorType {

			private ILRuntime.Runtime.Enviorment.AppDomain appdomain;
			private ILTypeInstance instance;

			public IListAdaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance) {
				this.appdomain = appdomain;
				this.instance = instance;
			}
			public ILTypeInstance ILInstance { get { return instance; } set { instance = value; } }
			// public ILRuntime.Runtime.Enviorment.AppDomain AppDomain { get { return appdomain; } set { appdomain = value; } }

			// public int Count {
            //     get {
            //         return 0;
            //     }
            // }
			// public bool IsReadOnly {
            //     get {
            //         return true;
            //     }
            // }
			// object IList<object>.this[int index] {
			// 	get { return this; }
			// 	set { }
			// }

			int ICollection<ILTypeInstance>.Count {
                get {
                    return 0;
                }
            }
			bool ICollection<ILTypeInstance>.IsReadOnly  {
                get {
                    return true;
                }
            } 

			// IMethod mIndexOfmd;
            // public int IndexOf(object item) {
            //     // if (mIndexOfmd == null) 
            //     //     mIndexOfmd == instance.Type.GetMethod("IndexOf", ?);
            //     // if (mIndexOfmd != null) 
            //     //     return (int)appdomain.Invoke(mIndexOfmd, instance);
            //     return -1;
            // }
            // IMethod mInsertmd;
			// public void Insert(int index, object item) { // 当有多个参数时,怎么调用比较优化一点儿?
            //     // if (mInsertmd == null) 
            //     //     mInsertmd == instance.type.GetMethod("Insert", );
            //     // if (mInsertmd != null) 
            //     //     appdomain.Invoke(mInsertmd, instance);
			// }

			public void RemoveAt(int index) {
            }

			public void Add(object item) {
            }
			public bool Contains(object item) {
                return true;
            }
			public void CopyTo(object[] array, int arrayIndex) {
            }
			public bool Remove(object item) {
                return true;
            }
			//IEnumerator<object> IEnumerable<object>.GetEnumerator() {
			//	return null;
   //         }

			public void Clear() {
			}
			public IEnumerator GetEnumerator() {
				return null;
                
            }

			int IList<ILTypeInstance>.IndexOf(ILTypeInstance item)
                {
                    throw new NotImplementedException();
                }

			ILTypeInstance IList<ILTypeInstance>.this[int index] {
                get {
					return null;
                }
                set {
                }
            }

			void IList<ILTypeInstance>.Insert(int index, ILTypeInstance item) {
            }
			void IList<ILTypeInstance>.RemoveAt(int index) {
            }
			void ICollection<ILTypeInstance>.Add(ILTypeInstance item) {
            }
			void ICollection<ILTypeInstance>.Clear() {
            }
			bool ICollection<ILTypeInstance>.Contains(ILTypeInstance item) {
                return true;
            }
			void ICollection<ILTypeInstance>.CopyTo(ILTypeInstance[] array, int arrayIndex) {
            }
			bool ICollection<ILTypeInstance>.Remove(ILTypeInstance item) {
                return true;
            }
			IEnumerator<ILTypeInstance> IEnumerable<ILTypeInstance>.GetEnumerator() {
                return null;
            }
			IEnumerator IEnumerable.GetEnumerator() {
                return null;
            }
		}
	}
}
