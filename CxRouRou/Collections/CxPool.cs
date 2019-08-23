using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Collections
{
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CxPool<T>
    {
        /// <summary>
        /// 对象池
        /// </summary>
        private readonly Queue<T> _pool;
        /// <summary>
        /// 构造器
        /// </summary>
        private readonly Func<T> _ctor;
        /// <summary>
        /// 重置方法
        /// </summary>
        private readonly Action<T> _reset;
        /// <summary>
        /// 获取一个T对象
        /// </summary>
        private T NewT => _ctor == null ? default(T) : _ctor.Invoke();
        /// <summary>
        /// 对象池当前大小
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _pool.Count;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="size">初始化大小</param>
        /// <param name="ctor">构造器</param>
        /// <param name="reset">重置方法</param>
        public CxPool(int size, Func<T> ctor = null, Action<T> reset = null)
        {
            _ctor = ctor;
            _reset = reset;
            _pool = new Queue<T>(size);
            for (int i = 0; i < size; i++)
            {
                _pool.Enqueue(NewT);
            }
        }
        /// <summary>
        /// 线程安全对象池
        /// </summary>
        /// <param name="size"></param>
        /// <param name="ctor"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static CxPool<T> ThreadSafePool(int size, Func<T> ctor = null, Action<T> reset = null)
        {
            return new CxThreadSafePool<T>(size, ctor, reset);
        }
        /// <summary>
        /// 取得一个对象
        /// </summary>
        /// <returns></returns>
        public virtual T Pop()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
            return NewT;
        }
        /// <summary>
        /// 回收一个对象
        /// </summary>
        /// <param name="t"></param>
        public virtual void Push(T t)
        {
            if (t == null)
            {
                return;
            }
            if (_reset != null)
            {
                //重置对象，以便下次使用
                _reset.Invoke(t);
            }
            _pool.Enqueue(t);
        }
    }
    /// <summary>
    /// 对象池(线程安全)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CxThreadSafePool<T> : CxPool<T>
    {
        readonly object _syncObject = new object();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="size"></param>
        /// <param name="ctor"></param>
        /// <param name="_reset"></param>
        public CxThreadSafePool(int size, Func<T> ctor = null, Action<T> _reset = null) : base(size, ctor, _reset)
        {
        }
        /// <summary>
        /// 取得一个对象
        /// </summary>
        /// <returns></returns>
        public override T Pop()
        {
            lock (_syncObject)
            {
                return base.Pop();
            }
        }
        /// <summary>
        /// 回收一个对象
        /// </summary>
        /// <param name="t"></param>
        public override void Push(T t)
        {
            lock (_syncObject)
            {
                base.Push(t);
            }
        }
        /// <summary>
        /// 对象个数
        /// </summary>
        public override int Count
        {
            get
            {
                lock (_syncObject)
                {
                    return base.Count;
                }
            }
        }
    }
}
