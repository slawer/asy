using System;
using System.Collections.Generic;

namespace WCF
{
    /// <summary>
    /// Реализует зарегистрированого пользователя системы
    /// </summary>
    public class User
    {
        protected Role role;            // роль пользователя в системе
        protected Handle handle;        // идентификатор пользователя

        protected ICallBack user;       // интерфей для взаимодействия с пользователем
        
        protected bool selected;        // определяет все параметры передавать пользователю или на выбор
        protected List<int> indexes;    // индексы требуемых параметров клиенту

        protected UserMode mode;        // режим в котором работает пользователь

        /// <summary>
        /// Инициализирует новый экземпляр класса
        /// </summary>
        protected User()
        {
            handle = new Handle();
                        
            selected = false;
            mode = UserMode.Active;
            
            indexes = new List<int>();
        }

        /// <summary>
        /// Инициализировать нового пользователя
        /// </summary>
        /// <param name="UserInterface">Интерфейс для взаимодействия с пользователем</param>
        /// <param name="UserRole">Роль пользователя в системе</param>
        /// <returns>Новый пользователь системы</returns>
        public static User InstanceUser(ICallBack UserInterface, Role UserRole)
        {
            User user = new User();

            user.Role = UserRole;
            user.Interface = UserInterface;

            return user;
        }

        /// <summary>
        /// Определяет роль пользователя в системе
        /// </summary>
        public Role Role
        {
            get { return role; }
            set { role = value; }
        }

        /// <summary>
        /// Возвращяет описатель пользователя
        /// </summary>
        public Handle Handle
        {
            get { return handle; }
        }

        /// <summary>
        /// Определяет интерфейс с помощью которого осуществляется 
        /// взаимодействие с пользователем
        /// </summary>
        public ICallBack Interface
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// Определяет все параметры передавать пользователю или на выбор
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Определяет режим в котором работает пользователь
        /// </summary>
        public UserMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Возвращяет массив требуемых параметров пользователю
        /// </summary>
        public int[] Indexes
        {
            get { return indexes.ToArray(); }
        }

        /// <summary>
        /// Установить номера требуемых параметров
        /// </summary>
        /// <param name="Indexes">Спосик номеров, требуемых параметров</param>
        public void SetIndexes(int[] Indexes)
        {
            indexes.Clear();
            foreach (var index in Indexes)
            {
                if (index > -1 && index < 1024)
                {
                    if (!Exist(index))
                    {
                        indexes.Add(index);
                    }
                }
            }
        }

        /// <summary>
        /// Очистить номера требуемых параметров
        /// </summary>
        public void ClearIndexes()
        {
            indexes.Clear();
        }

        /// <summary>
        /// Проверить наличие индекса в массиве
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool Exist(int index)
        {
            foreach (int value in indexes)
            {
                if (value == index)
                {
                    return true;
                }
            }

            return false;
        }
    }
}