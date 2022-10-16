using System.Collections.Generic;
using Framework.Util;
using System;

namespace Framework.Core {

    public class PoolObjectFactory : Singleton<PoolObjectFactory>, IObjectFactory {

        public class ObjectPool {
            public readonly List<PoolData> _pool;
            public int Max {
                get;
                set;
            }
            public bool Limit {
                get;
                set;
            }
            public ObjectPool() {
                Limit = false;
                _pool = new List<PoolData>();
            }
        }
        public class PoolData {
            public bool InUse {
                get;
                set;
            }
            public object Obj {
                get;
                set;
            }
        }

        private readonly Dictionary<Type, ObjectPool> pool;
        public PoolObjectFactory() {
            pool = new Dictionary<Type, ObjectPool>();
        }

        public object AcquireObject(string classFullName) {
            Type type = GameApplication.Instance.HotFix.LoadType(classFullName);
            lock (pool) {
                if (pool.ContainsKey(type)) {
                    if (pool[type]._pool.Count > 0) {
                        for (int i = 0; i < pool[type]._pool.Count; i++) {
                            var p = pool[type]._pool[i];
                            if (!p.InUse) {
                                p.InUse = true;
                                return p.Obj;
                            }
                        }
                    }
                    if (pool[type].Limit && pool[type]._pool.Count >= pool[type].Max) {
                        throw new Exception("max limit is arrived.");
                    }
                }
                object obj = GameApplication.Instance.HotFix.CreateInstance(classFullName);
                var poolData = new PoolData {
                    InUse = true,
                    Obj = obj
                };
                if (!pool.ContainsKey(type)) {
                    ObjectPool objPool = new ObjectPool();
                    pool.Add(type, objPool);
                }
                pool[type]._pool.Add(poolData);
                return obj;
            }
        }

        public void ReleaseObject(object obj) {
        }
    }
}
