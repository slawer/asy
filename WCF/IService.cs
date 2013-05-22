using System;
using System.ServiceModel;

namespace WCF
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract(CallbackContract=typeof(ICallBack), SessionMode = SessionMode.Required)]
    public interface IService
    {
        // ---- команды управления пользователем ----

        /// <summary>
        /// Зарегистрироваться в системе
        /// </summary>
        /// <param name="Role">Требуемая роль в системе</param>
        /// <returns>Описатель используемый при взаимодействии в системе в дальнейшем</returns>
        [OperationContract]
        Handle Register(Role Role);

        /// <summary>
        /// Установить режим работы
        /// </summary>
        /// <param name="mode">Требуемый режим работы</param>
        /// <param name="handle">Описатель пользователя для которого установливается режим работы</param>
        [OperationContract(IsOneWay = true)]
        void SetMode(Handle handle, UserMode mode);

        /// <summary>
        /// Получить текущий режим работы пользователя
        /// </summary>
        /// <param name="handle">Описатель пользователя</param>
        /// <returns>Текущий режим работы пользователя</returns>
        [OperationContract()]
        UserMode GetCurrentMode(Handle handle);

        // ---- команды управления конфигурацией параметров пользователя ----

        /// <summary>
        /// Установить параметры, которые необходимо передавать
        /// </summary>
        /// <param name="handle">Идентификатор пользователя</param>
        /// <param name="Indexes">Массив индексов параметров</param>
        [OperationContract(IsOneWay = true)]
        void SelectParameters(Handle handle, int[] Indexes);

        /// <summary>
        /// Очистить список параметров для отправки пользователю
        /// </summary>
        /// <param name="handle">Описатель пользователя</param>
        [OperationContract(IsOneWay = true)]
        void ClearSelectedParameters(Handle handle);

        /// <summary>
        /// Получить список параметров с описаниями и номерами в списке
        /// </summary>
        /// <returns>Список имеющихся параметров</returns>
        [OperationContract()]
        PDescription[] GetParametersDescription();

        /// <summary>
        /// Присвоить параметру указанное значение
        /// </summary>
        /// <param name="handle">Описатель пользователя</param>
        /// <param name="Index">Номер параметра в списке</param>
        /// <param name="Value">Значение которое присвоить параметру</param>
        [OperationContract(IsOneWay = true)]
        void SetParameterValue(Handle handle, Int32 Index, Single Value);

        // ---- команды отправки пакета в COM порт ----

        /// <summary>
        /// Отправить пакет в порт
        /// </summary>
        /// <param name="packet">Пакет для отправки в COM порт. Версия TCP протокола обмена</param>
        [OperationContract(IsOneWay = true)]
        void ToCOM(string packet);

        /// <summary>
        /// Запросить срез данных. Асинхронный метод
        /// </summary>
        /// <param name="handle">Дескриптор пользователя</param>
        [OperationContract(IsOneWay = true)]
        void RequestDataSlice(Handle handle);

        /// <summary>
        /// Запросить срез данных. Синхронный метод
        /// </summary>
        /// <param name="handle">Дескриптор  пользователя</param>
        /// <returns>Срез данных</returns>
        [OperationContract()]
        Single[] GetDataSlice(Handle handle);
    }
}