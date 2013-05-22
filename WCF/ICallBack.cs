using System;
using System.ServiceModel;

namespace WCF
{
    public interface ICallBack
    {
        /// <summary>
        /// Передать пользователю весь срез данных
        /// </summary>
        /// <param name="Time">Время получения среза данных</param>
        /// <param name="Slice">Срез данных</param>
        [OperationContract(IsOneWay = true)]
        void SendAll(DateTime Time, Single[] Slice);
    }
}