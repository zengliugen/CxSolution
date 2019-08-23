using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using CxSolution.CxRouRou.Collections;

namespace CxSolution.CxRouRou.Net.Sockets.Tcp
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    internal class CxSessionManager
    {
        /// <summary>
        /// 当前ID
        /// </summary>
        public uint _currentID;
        /// <summary>
        /// 会话池
        /// </summary>
        private readonly CxPool<CxSession> _sessionPool;
        /// <summary>
        /// 在线列表
        /// </summary>
        private readonly Dictionary<uint, CxSession> _onlines;
        /// <summary>
        /// 异步锁
        /// </summary>
        private object _syncLock = new object();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="poolSize"></param>
        /// <param name="receiveBufferSize"></param>
        /// <param name="saeaPool"></param>
        public CxSessionManager(int poolSize, int receiveBufferSize, CxPool<SocketAsyncEventArgs> saeaPool)
        {
            _sessionPool = new CxPool<CxSession>(poolSize, () =>
            {
                return new CxSession(receiveBufferSize, saeaPool);
            }, (session) =>
            {
                session.Clear();
            });
            _onlines = new Dictionary<uint, CxSession>(poolSize);
        }
        /// <summary>
        /// 取得一个会话
        /// </summary>
        /// <returns></returns>
        internal CxSession Pop()
        {
            lock (_syncLock)
            {
                CxSession session = _sessionPool.Pop();
                if (++_currentID == 0) _currentID = 1;
                while (_onlines.ContainsKey(_currentID))
                {
                    ++_currentID;
                }
                session.ID = _currentID;
                _onlines[_currentID] = session;
                return session;
            }
        }
        /// <summary>
        /// 回收一个会话
        /// </summary>
        /// <param name="id"></param>
        /// <param name="session"></param>
        internal void Push(uint id, CxSession session)
        {
            if (session == null)
            {
                return;
            }
            lock (_syncLock)
            {
                _sessionPool.Push(session);
                _onlines.Remove(id);
            }
        }
        /// <summary>
        /// 取得一个在线会话
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal CxSession GetOnline(uint id)
        {
            lock (_syncLock)
            {
                if (_onlines.TryGetValue(id, out CxSession session))
                {
                    return session;
                }
            }
            return null;
        }
        /// <summary>
        /// 取得所有在线会话
        /// </summary>
        /// <returns></returns>
        internal ICollection<CxSession> GetAllOnline()
        {
            lock (_syncLock)
            {
                return _onlines.Values;
            }
        }
        /// <summary>
        /// 当前连接数
        /// </summary>
        internal int OnlineNum
        {
            get
            {
                lock (_syncLock)
                {
                    return _onlines.Count;
                }
            }
        }
    }
}
