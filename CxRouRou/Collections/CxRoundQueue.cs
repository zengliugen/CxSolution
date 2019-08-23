using System;
using System.Collections.Generic;
using System.Text;

namespace CxSolution.CxRouRou.Collections
{
    /// <summary>
    /// 循环队列
    /// </summary>
    public sealed class CxRoundQueue<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        private T[] _data;
        /// <summary>
        /// 索引
        /// </summary>
        private int _index;
        /// <summary>
        /// 初始化
        /// </summary>
        public CxRoundQueue(int size, Func<T> ctor)
        {
            if (size <= 0)
            {
                throw new ArgumentException("size参数值必须大于0", "size");
            }
            if (ctor == null)
            {
                throw new ArgumentNullException("ctor", "必须提供构造方法ctor");
            }
            _data = new T[size];
            _index = 0;
            for (int i = 0; i < size; i++)
            {
                _data[i] = ctor.Invoke();
            }
        }
        /// <summary>
        /// 使用参数包装
        /// </summary>
        /// <param name="colletion"></param>
        public CxRoundQueue(T[] colletion)
        {
            _data = colletion ?? throw new ArgumentNullException("colletion", "参数不可为空");
            _index = 0;
        }
        /// <summary>
        /// 返回当前索引元素，并将索引指向下一个元素
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            if (_data == null)
            {
                throw new InvalidOperationException();
            }
            T t = _data[_index];
            _index = (_index + 1) % _data.Length;
            return t;
        }
        /// <summary>
        /// 返回当前索引元素
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {

            if (_data == null)
            {
                throw new InvalidOperationException();
            }
            return _data[_index];
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            _data = null;
            _index = 0;
        }
        /// <summary>
        /// 获取元素个数
        /// </summary>
        public int Count
        {
            get
            {
                return _data == null ? 0 : _data.Length;
            }
        }
        /// <summary>
        /// 支持foreach操作
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (_data == null)
            {
                throw new ObjectDisposedException("");
            }
            return ((IEnumerable<T>)_data).GetEnumerator();
        }
    }
}
