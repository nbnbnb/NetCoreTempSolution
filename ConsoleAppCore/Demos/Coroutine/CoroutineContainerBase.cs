using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCore.Demos.Coroutine
{

    class CoroutineContainerBase : ICoroutineContainer
    {
        /// <summary>
        /// 存储协程单元的列表
        /// </summary>
        private List<UnitItem> _units = new List<UnitItem>();

        /// <summary>
        /// 存储注册的协程单元，与协程单元列表分开，实现注册与执行互不影响
        /// </summary>
        private List<UnitItem> _addUnits = new List<UnitItem>();

        /// <summary>
        /// 错误处理
        /// </summary>
        private Action<ICoroutineUnit, Exception> _errorHandler;

        public CoroutineContainerBase(Action<ICoroutineUnit, Exception> errorHandler)
        {
            _errorHandler = errorHandler;
        }

        public void Register(ICoroutineUnit unit)
        {
            lock (_addUnits)
            {
                _addUnits.Add(new UnitItem { Unit = unit, UnitResult = null });
            }
        }

        public void Run()
        {
            // 开启一个单独任务执行
            Task.Run(() =>
            {
                while (true)
                {
                    // 将注册的协程单元加入到列表
                    lock (_addUnits)
                    {
                        foreach (var tp in _addUnits)
                        {
                            _units.Add(tp);
                        }
                        _addUnits.Clear();
                    }

                    // 记录处理失败的协程单元
                    UnitItem errorUnitItem = null;

                    // 依次处理协程单元
                    foreach (var tp in _units)
                    {
                        errorUnitItem = null;
                        // 协程单元还没有开始
                        if (tp.UnitResult == null)
                        {
                            // 获取协程单元中要处理的迭代器
                            var result = tp.Unit.Do();

                            // 运行到下一个断点
                            try
                            {
                                // 处理迭代器中的第一项
                                result.MoveNext();
                            }
                            catch (Exception ex)
                            {
                                _errorHandler(tp.Unit, ex);
                                //_units.Remove(tp);  // TODO: Error
                                errorUnitItem = tp;
                                break;
                            }

                            // 只要处理过依次迭代
                            // 则将要处理的迭代器存储起来
                            tp.UnitResult = result;
                        }
                        else  // 协程单元已经发起过一次 MoveNext 了
                        {
                            // 检查等待是否已经完成，如果已完成则继续执行
                            if (tp.UnitResult.Current.IsCanceled
                            || tp.UnitResult.Current.IsCompleted
                            || tp.UnitResult.Current.IsFaulted)
                            {
                                var nextResult = true;

                                // DEBUG 
                                if (tp.UnitResult.Current is Task<HttpResponseMessage>)
                                {
                                    var resposne = tp.UnitResult.Current as Task<HttpResponseMessage>;
                                    var content = resposne.Result.Content.ReadAsByteArrayAsync().Result;
                                    Console.WriteLine($"Url:{resposne.Result.RequestMessage.RequestUri}: Size:{content.Length}");
                                }

                                try
                                {
                                    // 继续获取迭代器中的内容
                                    nextResult = tp.UnitResult.MoveNext();
                                }
                                catch (Exception ex)
                                {
                                    _errorHandler(tp.Unit, ex);
                                    //_units.Remove(tp);  // TODO: Error
                                    errorUnitItem = tp;
                                    break;
                                }

                                // 迭代器中没有更多的元素了
                                // 将其移除
                                if (!nextResult)
                                {
                                    //_units.Remove(tp);  // TODO: Error
                                    errorUnitItem = tp;
                                    break;
                                }
                            }
                        }
                    }

                    // 在 foreach 的协程单元发生了异常
                    // 将其移除
                    if (errorUnitItem != null)
                    {
                        _units.Remove(errorUnitItem);
                    }

                    if (_units.Count == 0)
                    {
                        lock (_addUnits)
                        {
                            if (_addUnits.Count == 0)
                            {
                                // 没有更多项目了
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine();
                Console.WriteLine("---执行完成---");
            });
        }
    }
}
